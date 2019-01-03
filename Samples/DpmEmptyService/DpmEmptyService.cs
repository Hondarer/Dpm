using Hondarersoft.Dpm.ServiceProcess;
using System.Collections.Generic;

namespace Hondarersoft.Dpm.Samples
{
    public class DpmEmptyService : DpmServiceBase
    {
        public static int Main(string[] args)
        {
            DpmEmptyService instance = new DpmEmptyService();

            #region Region for self install

            ServiceInstallParameter serviceInstallParameter = new ServiceInstallParameter
            {
                // DisplayName and Description are automatically obtained from AssemblyInfo.cs.
                ExecutableUsers = new List<string>() { "Users" }
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
