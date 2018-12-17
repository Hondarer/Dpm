using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Collections.Generic;

namespace Hondarersoft.Dpm.Samples
{
    public class DpmRemoteCommandService : DpmServiceBase
    {
        public enum RemoteCommands : int
        {
            Unkhown = 0,
            SendStringMessage = 1
        }

        public static int Main(string[] args)
        {
            DpmRemoteCommandService instance = new DpmRemoteCommandService();

            #region Region for self install

            ServiceInstallParameter serviceInstallParameter = new ServiceInstallParameter
            {
                DisplayName = "IpcRemoteCommand service",
                Description = "Sample service for using IpcRemoteCommand",
                ExecutableUsers = new List<string>() { "Everyone" }
            };

            if (instance.TryInstall(serviceInstallParameter) == true)
            {
                return instance.ExitCode;
            }

            #endregion

            Run(instance);

            return instance.ExitCode;
        }

        public DpmRemoteCommandService() : base()
        {
            // Choice None, Ipc, Tcp, Both (Default: None)
            RemoteCommandSupport = RemoteCommandSupports.Both;

            // For Tcp
            TcpServicePort = 19000;
        }

        protected override object OnRemoteCommand(object sender, RemoteCommandEventArgs eventArgs)
        {
            RemoteCommands command = (RemoteCommands)Enum.ToObject(typeof(RemoteCommands), eventArgs.Command);

            switch (command)
            {
                case RemoteCommands.SendStringMessage:
                    EventLog.WriteEntry($"OnRemoteCommand: \"{eventArgs.Data.ToString()}\"");
                    break;
                default:
                    // NOP
                    break;
            }

            return base.OnRemoteCommand(sender, eventArgs);
        }
    }
}
