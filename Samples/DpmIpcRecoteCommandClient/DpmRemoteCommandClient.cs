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
            Remoting.InitializeIpcClientChannel();
            RemoteCommandService client = Remoting.GetIpcRemoteClient<RemoteCommandService>(nameof(DpmRemoteCommandService));

            Console.WriteLine("*** DpmRemoteCommandClient ***\r\n");
            Console.Write("Input message: ");

            string input = Console.ReadLine();
            Console.WriteLine();

            try
            {
                client.RemoteCommand((int)DpmRemoteCommandService.RemoteCommands.SendStringMessage, input);

                Console.WriteLine("> Done.");
            }
            catch (RemotingException ex)
            {
                Console.Error.WriteLine(ex);
            }



            try
            {
                RemoteCommandService tcpclient = (RemoteCommandService)Activator.GetObject(typeof(RemoteCommandService), $"tcp://localhost:19000/{nameof(RemoteCommandService)}");
                tcpclient.RemoteCommand((int)DpmRemoteCommandService.RemoteCommands.SendStringMessage, input);

                Console.WriteLine("> Done.");
            }
            catch (RemotingException ex)
            {
                Console.Error.WriteLine(ex);
            }

        }
    }
}
