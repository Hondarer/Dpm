using Hondarersoft.Dpm.ServiceProcess;
using Hondarersoft.Dpm.Timer;
using System.Collections.Generic;

namespace Hondarersoft.Dpm.Services
{
    public class DpmTimerManager : TimerManagerCore
    {
        public static int Main(string[] args)
        {
            DpmTimerManager instance = new DpmTimerManager();

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
