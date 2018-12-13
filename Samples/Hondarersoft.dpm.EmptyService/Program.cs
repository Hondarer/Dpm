using Hondarersoft.Dpm.ServiceProcess;
using System.Collections.Generic;

namespace Hondarersoft.Dpm
{
    public class DpmEmptyService : DpmServiceBase
    {
        public static int Main(string[] args)
        {
            DpmEmptyService instance = new DpmEmptyService();

            ServiceInstallParameter serviceInstallParameter = new ServiceInstallParameter
            {
                DisplayName = "Empty Service",
                Description = "Description of Empty Service",
                ExecutableUsers = new List<string>() { "Everyone" } // TODO: 設定時の例外補足が甘い気がする
            };

            if (instance.TryInstall(serviceInstallParameter) == true)
            {
                return instance.ExitCode;
            }

            Run(instance);

            return instance.ExitCode;
        }

        public DpmEmptyService() : base()
        {
            //CanStop = false; // The default is true.
            //AutoLog = false; // The default is true.
            //SupportInstanceID = true; // The default is false.
            RemoteCommandSupport = RemoteCommandSupports.Ipc;
        }

        protected override object OnRemoteCommand(object sender, RemoteCommandEventArgs eventArgs)
        {
            EventLog.WriteEntry("OnRemoteCommand " + eventArgs.Data.ToString());

            return base.OnRemoteCommand(sender, eventArgs);
        }
    }
}
