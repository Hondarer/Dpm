#define USE_IPC

using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    public class AreaManagerCore : DpmServiceBase
    {

#if USE_IPC
        private IpcServerChannel serverChannel;
#else
        private TcpChannel serverChannel;
#endif

        protected override void OnStart(string[] args)
        {
#if USE_IPC
            IDictionary props = new Hashtable
            {
                ["name"] = AreaManagerService.SERVICE_NAME,
                ["portName"] = AreaManagerService.SERVICE_NAME,
                ["authorizedGroup"] = "Everyone"
            };
            serverChannel = new IpcServerChannel(props, null, null);
            ChannelServices.RegisterChannel(serverChannel, true);
            AreaManagerService areaManagerService = new AreaManagerService();
            RemotingServices.Marshal(areaManagerService, nameof(AreaManagerService));
#else
            serverChannel = new TcpChannel(AreaManagerService.SERVICE_PORT); // ポート番号に 0 を渡すと、動的割り当てができる。
            ChannelServices.RegisterChannel(channel, false);
            AreaManagerService areaManagerService = new AreaManagerService();
            RemotingServices.Marshal(areaManagerService, nameof(AreaManagerService));
#endif

            areaManagerService.OnTestCommand += AreaManagerService_OnTestCommand;

#if !USE_IPC
            serverChannel.StartListening(null);
#endif

            base.OnStart(args);
        }

        private void AreaManagerService_OnTestCommand(object sender, AreaManagerService.TestCommandEventArgs eventArgs)
        {
            EventLog.WriteEntry($"OnTestCommand: {eventArgs.Command}");
        }

        protected override void OnStop()
        {
#if !USE_IPC
            serverChannel.StopListening(null);
#endif

            ExitCode = 0;

            base.OnStop();
        }
    }

    public class AreaManagerService : MarshalByRefObject
    {
#if USE_IPC
        public const string SERVICE_NAME = "DpmAreaManager";
#else
        public const int SERVICE_PORT = 19000;
#endif

        private static AreaManagerService client;

        public static AreaManagerService Client
        {
            get
            {
                if (client == null)
                {
#if USE_IPC
                    client = (AreaManagerService)Activator.GetObject(typeof(AreaManagerService), $"ipc://{SERVICE_NAME}/{nameof(AreaManagerService)}");
#else
                    client = (AreaManagerService)Activator.GetObject(typeof(AreaManagerService), $"tcp://localhost:{SERVICE_PORT}/{nameof(AreaManagerService)}");
#endif
                }
                return client;
            }
        }

        public class TestCommandEventArgs : EventArgs
        {
            public string Command { get; set; }
        }

        public delegate void TestCommandEventHandler(object sender, TestCommandEventArgs eventArgs);
        public event TestCommandEventHandler OnTestCommand;

        public void TestCommand(string command)
        {
            if (OnTestCommand != null)
            {
                OnTestCommand(this, new TestCommandEventArgs() { Command = command });
            }

            return;
        }

        public override object InitializeLifetimeService()
        {
            // オブジェクトのリース期限を無期限にする
            return null;
        }
    }
}
