using System.Configuration.Install;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public abstract class SelfServiceInstallerBase : Installer
    {
        public SelfServiceInstallerBase()
        {
            ServiceProcessInstaller spi = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            ConfigurableServiceInstaller si = new ConfigurableServiceInstaller
            {
                ServiceName = InstallableServiceBase.ServiceName,
                DisplayName = InstallableServiceBase.DisplayName,
                Description = InstallableServiceBase.Description,

                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(spi);
            Installers.Add(si);
        }
    }
}
