using Hondarersoft.Dpm.Apis;
using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Runtime.Remoting;

namespace Hondarersoft.Dpm.Samples
{
    class DpmIpcRecoteCommandClient
    {
        private static void Main(string[] args)
        {
            Remoting.InitializeIpcClientChannel();
            RemoteCommandService client = Remoting.GetIpcRemoteClient<RemoteCommandService>(nameof(DpmIpcRemoteCommandService));

            Console.WriteLine("*** DpmIpcRecoteCommandClient ***\r\n");
            Console.Write("Input message: ");

            string input = Console.ReadLine();
            Console.WriteLine();

            try
            {
                client.RemoteCommand((int)DpmIpcRemoteCommandService.RemoteCommands.SendStringMessage, input);

                Console.WriteLine("> Done.");
            }
            catch (RemotingException ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
