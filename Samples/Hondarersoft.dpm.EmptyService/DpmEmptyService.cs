using Hondarersoft.Dpm.ServiceProcess;
using System.Collections.Generic;

namespace Hondarersoft.Dpm
{
    public class DpmEmptyService : DpmServiceBase
    {
        public static int Main(string[] args)
        {
            DpmEmptyService instance = new DpmEmptyService();

            #region Region for self install

            ServiceInstallParameter serviceInstallParameter = new ServiceInstallParameter
            {
                DisplayName = "Empty service",
                Description = "Sample of empty service",
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
    }
}
