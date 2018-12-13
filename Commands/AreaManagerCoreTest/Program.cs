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
                InitializeIpcClientChannel();
#endif

                RemoteCommandService client = GetIpcRemoteClient<RemoteCommandService>("DpmEmptyService");

                client.RemoteCommand(1, "ABCD");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception:\r\n{ex}");
            }
        }

        static void InitializeIpcClientChannel()
        {
            IpcClientChannel clientChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(clientChannel, true);
        }

        static T GetIpcRemoteClient<T>(string channelName, string objectName=null)
        {
            if (objectName == null)
            {
                objectName = typeof(T).Name;
            }
            return (T)Activator.GetObject(typeof(T), $"ipc://{channelName}/{objectName}");
        }
    }
}
