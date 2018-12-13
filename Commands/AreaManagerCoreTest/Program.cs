#define USE_IPC

using Hondarersoft.Dpm.Areas;
using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading.Tasks;

namespace AreaManagerCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
#if USE_IPC
                IpcClientChannel clientChannel = new IpcClientChannel();
                ChannelServices.RegisterChannel(clientChannel, true);
#endif

                RemoteCommandService client = (RemoteCommandService)Activator.GetObject(typeof(RemoteCommandService), $"ipc://DpmEmptyService/RemoteCommandService");
                client.RemoteCommand(1, "ABCD");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception:\r\n{ex}");
            }
        }
    }
}
