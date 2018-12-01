using System.Configuration.Install;
using System.ServiceProcess;

namespace Hondarersoft.Dpm
{
    public abstract class ServiceInstallerBase : Installer
    {
        public ServiceInstallerBase()
        {
            ServiceProcessInstaller spi = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            ServiceInstaller si = new ServiceInstaller
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
