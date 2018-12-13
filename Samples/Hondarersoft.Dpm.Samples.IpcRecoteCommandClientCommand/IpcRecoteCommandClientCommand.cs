using Hondarersoft.Dpm.Apis;
using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Runtime.Remoting;

namespace Hondarersoft.Dpm.Samples
{
    class IpcRecoteCommandClientCommand
    {
        private static void Main(string[] args)
        {
            Remoting.InitializeIpcClientChannel();
            RemoteCommandService client = Remoting.GetIpcRemoteClient<RemoteCommandService>(nameof(IpcRemoteCommandService));

            Console.WriteLine("*** IpcRecoteCommandClientCommand ***\r\n");
            Console.Write("Input message: ");

            string input = Console.ReadLine();
            Console.WriteLine();

            try
            {
                client.RemoteCommand((int)IpcRemoteCommandService.RemoteCommands.SendStringMessage, input);

                Console.WriteLine("> Success.");
            }
            catch (RemotingException ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
