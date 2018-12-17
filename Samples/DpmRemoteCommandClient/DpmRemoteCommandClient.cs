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
            RemoteCommandService tcpClient = (RemoteCommandService)Activator.GetObject(typeof(RemoteCommandService), $"tcp://localhost:19000/{nameof(RemoteCommandService)}");

            Console.WriteLine("*** DpmRemoteCommandClient ***\r\n");
            Console.Write("Input message: ");

            string input = Console.ReadLine();
            Console.WriteLine();

            try
            {
                ipcClient.RemoteCommand((int)DpmRemoteCommandService.RemoteCommands.SendStringMessage, input);

                Console.WriteLine("> Ipc done.");
            }
            catch (RemotingException ex)
            {
                Console.Error.WriteLine(ex);
            }

            try
            {
                tcpClient.RemoteCommand((int)DpmRemoteCommandService.RemoteCommands.SendStringMessage, input);

                Console.WriteLine("> Tcp done.");
            }
            catch (RemotingException ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
