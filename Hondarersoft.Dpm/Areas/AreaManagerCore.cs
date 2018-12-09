using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    public class AreaManagerCore : DpmServiceBase
    {
        public const string SERVICE_NAME = "DpmAreaManager";

        private TcpChannel channel;

        protected override void OnStart(string[] args)
        {
            channel = new TcpChannel(AreaManagerService.SERVICE_PORT); // ポート番号に 0 を渡すと、動的割り当てができる。
            ChannelServices.RegisterChannel(channel, false);
            AreaManagerService areaManagerService = new AreaManagerService();
            RemotingServices.Marshal(areaManagerService, nameof(AreaManagerService));

            areaManagerService.OnTestCommand += AreaManagerService_OnTestCommand;

            channel.StartListening(null);

            base.OnStart(args);
        }

        private void AreaManagerService_OnTestCommand(object sender, AreaManagerService.TestCommandEventArgs eventArgs)
        {
            EventLog.WriteEntry($"OnTestCommand: {eventArgs.Command}");
        }

        protected override void OnStop()
        {
            channel.StopListening(null);

            ExitCode = 0;

            base.OnStop();
        }
    }

    public class AreaManagerService : MarshalByRefObject
    {
        public const int SERVICE_PORT = 19000;

        private static AreaManagerService client;

        public static AreaManagerService Client
        {
            get
            {
                if (client == null)
                {
                    client = (AreaManagerService)Activator.GetObject(typeof(AreaManagerService), $"tcp://localhost:{SERVICE_PORT}/{nameof(AreaManagerService)}");
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
