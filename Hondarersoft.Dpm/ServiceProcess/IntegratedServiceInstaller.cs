using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class IntegratedServiceInstaller
    {
        public void Install(ServiceInstallParameter parameter)
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = parameter.Account
            };

            ConfigurableServiceInstaller serviceInstaller = new ConfigurableServiceInstaller
            {
                Context = new InstallContext(string.Concat(Path.GetTempPath(), "Install_", parameter.ServiceName, ".log"), new string[] { $"/assemblypath={parameter.AssemblyPath}" }),
                Parent = serviceProcessInstaller,
                ServiceName = parameter.ServiceName,
                DisplayName = parameter.DisplayName,
                Description = parameter.Description,
                StartType = parameter.StartType
            };

            if (parameter.ServicesDependedOn != null)
            {
                serviceInstaller.ServicesDependedOn = parameter.ServicesDependedOn.ToArray();
            }

            if (string.IsNullOrEmpty(parameter.InstanceID) != true)
            {
                if(parameter.Args==null)
                {
                    parameter.Args = new List<string>();
                }

                parameter.Args.Insert(0, $"/InstanceID=\"{parameter.InstanceID}\"");
            }

            if (parameter.Args != null)
            {
                serviceInstaller.Context.Parameters["assemblypath"] =
                    $"\"{serviceInstaller.Context.Parameters["assemblypath"]}\" {string.Join(" ", parameter.Args.ToArray())}";
            }

            serviceInstaller.Context.LogMessage($"*** インストールを開始します。{DateTime.Now}");

            ListDictionary state = new ListDictionary();
            serviceInstaller.Install(state);

            serviceInstaller.Context.LogMessage($"*** インストールが終了しました。{DateTime.Now}");
        }

        public void Uninstall(ServiceInstallParameter parameter)
        {
            ConfigurableServiceInstaller serviceInstaller = new ConfigurableServiceInstaller
            {
                Context = new InstallContext(string.Concat(Path.GetTempPath(), "Uninstall_", parameter.ServiceName, ".log"), null),
                ServiceName = parameter.ServiceName
            };

            serviceInstaller.Context.LogMessage($"*** アンインストールを開始します。{DateTime.Now}");

            serviceInstaller.Uninstall(null);

            serviceInstaller.Context.LogMessage($"*** アンインストールが終了しました。{DateTime.Now}");
        }
    }
}
