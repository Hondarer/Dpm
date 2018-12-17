using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Collections.Generic;

namespace Hondarersoft.Dpm.Samples
{
    public class DpmIpcRemoteCommandService : DpmServiceBase
    {
        public enum RemoteCommands : int
        {
            Unkhown = 0,
            SendStringMessage = 1
        }

        public static int Main(string[] args)
        {
            DpmIpcRemoteCommandService instance = new DpmIpcRemoteCommandService();

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

        public DpmIpcRemoteCommandService() : base()
        {
            RemoteCommandSupport = RemoteCommandSupports.Ipc;
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
                    break;

            }

            return base.OnRemoteCommand(sender, eventArgs);
        }
    }
}
