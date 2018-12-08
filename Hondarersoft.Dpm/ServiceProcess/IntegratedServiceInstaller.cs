using Hondarersoft.Dpm.Areas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class IntegratedServiceInstaller
    {
        public void Install(ServiceInstallParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = parameter.Account
            };

            ServiceInstaller serviceInstaller = new ServiceInstaller
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

            SetFailureActions(parameter);

            serviceInstaller.Context.LogMessage($"*** インストールが終了しました。{DateTime.Now}");
        }

        public void Uninstall(ServiceInstallParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            ServiceInstaller serviceInstaller = new ServiceInstaller
            {
                Context = new InstallContext(string.Concat(Path.GetTempPath(), "Uninstall_", parameter.ServiceName, ".log"), null),
                ServiceName = parameter.ServiceName
            };

            serviceInstaller.Context.LogMessage($"*** アンインストールを開始します。{DateTime.Now}");

            serviceInstaller.Uninstall(null);

            serviceInstaller.Context.LogMessage($"*** アンインストールが終了しました。{DateTime.Now}");
        }

        protected virtual void SetFailureActions(ServiceInstallParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            const int MAX_ACTIONS = 3;
            int[] arrActions = new int[MAX_ACTIONS * 2];
            int actionsIndex = 0;

            arrActions[actionsIndex++] = (int)parameter.FirstFailureActionType;
            arrActions[actionsIndex++] = (int)parameter.FirstFailureActionDelay.TotalMilliseconds;
            arrActions[actionsIndex++] = (int)parameter.SecondFailureActionType;
            arrActions[actionsIndex++] = (int)parameter.SecondFailureActionDelay.TotalMilliseconds;
            arrActions[actionsIndex++] = (int)parameter.SubsequentFailureActionType;
            arrActions[actionsIndex++] = (int)parameter.SubsequentFailureActionDelay.TotalMilliseconds;

            using (CoTaskMemory buffer = new CoTaskMemory((actionsIndex / 2) * Marshal.SizeOf(typeof(uint)) * 2))
            {
                Marshal.Copy(arrActions, 0, buffer.Pointer, actionsIndex);
                PInvoke.SERVICE_FAILURE_ACTIONS sfa = new PInvoke.SERVICE_FAILURE_ACTIONS
                {
                    cActions = (uint)(actionsIndex / 2),
                    dwResetPeriod = parameter.DaysToResetFailureCount,
                    lpCommand = parameter.CommandlineOfFailure,
                    lpRebootMsg = parameter.RebootMessageOfFailure,
                    lpsaActions = buffer.Pointer
                };

                ServiceController sc = new ServiceController(parameter.ServiceName);

                bool success = PInvoke.ChangeServiceFailureActions(sc.ServiceHandle.DangerousGetHandle(), PInvoke.SERVICE_CONFIG_FAILURE_ACTIONS, ref sfa);

                if (success == false)
                {
                    if (Marshal.GetLastWin32Error() == PInvoke.ERROR_ACCESS_DENIED)
                    {
                        throw new Exception("Access denied while setting failure actions.");
                    }
                    else
                    {
                        throw new Exception("Unknown error while setting failure actions.");
                    }
                }
            }
        }
    }
}
