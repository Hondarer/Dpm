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
                ExecutableUsers = new List<string>() { "everyone" }
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
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ExitCode = 0;

            base.OnStop();
        }
    }
}
