using Hondarersoft.Dpm.Apis;
using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Runtime.Remoting;

namespace Hondarersoft.Dpm.Samples
{
    class DpmRemoteCommandClient
    {
        private static void Main(string[] args)
        {
            // Initialize ipc channel
            Remoting.InitializeIpcClientChannel();
            RemoteCommandService ipcClient = Remoting.GetIpcRemoteClient<RemoteCommandService>(nameof(DpmRemoteCommandService));

            // Initialize tcp channel
            RemoteCommandService tcpClient = Remoting.GetTcpRemoteClient<RemoteCommandService>(null, DpmRemoteCommandService.SERVICE_PORT);

            Console.WriteLine("*** DpmRemoteCommandClient ***\r\n");
            Console.Write("Input message: ");

            string input = Console.ReadLine();
            Console.WriteLine();

            try
            {
                ipcClient.RemoteCommand((int)DpmRemoteCommandService.RemoteCommands.SendStringMessage, $"{input} from ipc");

                Console.WriteLine("> Ipc done.");
            }
            catch (RemotingException ex)
            {
                Console.Error.WriteLine(ex);
            }

            try
            {
                tcpClient.RemoteCommand((int)DpmRemoteCommandService.RemoteCommands.SendStringMessage, $"{input} from tcp");

                Console.WriteLine("> Tcp done.");
            }
            catch (RemotingException ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
