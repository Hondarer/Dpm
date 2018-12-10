#define USE_IPC

using Hondarersoft.Dpm.Areas;
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
                AreaManagerService.Client.TestCommand("ABCDE");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception:\r\n{ex}");
            }
        }
    }
}
