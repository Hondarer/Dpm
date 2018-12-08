using Hondarersoft.Dpm.Areas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class IntegratedServiceInstaller
    {
        public void Install(string serviceBaseName, string instanceID, ServiceInstallParameter parameter)
        {
            if (string.IsNullOrEmpty(serviceBaseName) == true)
            {
                throw new ArgumentException(nameof(serviceBaseName));
            }
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            ServiceIdentify serviceIdentify = new ServiceIdentify()
            {
                ServiceBaseName = serviceBaseName,
                InstanceID = instanceID
            };

            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = parameter.Account
            };

            string displayName = parameter.DisplayName;
            if(string.IsNullOrEmpty(displayName)==true)
            {
                displayName = serviceIdentify.ServiceName;
            }

            if (string.IsNullOrEmpty(serviceIdentify.InstanceID) != true)
            {
                displayName = string.Concat(displayName, " - ", serviceIdentify.InstanceID);
            }

            string description = parameter.Description;
            if (string.IsNullOrEmpty(description) == true)
            {
                description = serviceIdentify.ServiceName;
            }

            if (string.IsNullOrEmpty(serviceIdentify.InstanceID) != true)
            {
                description = string.Concat(description, " - ", serviceIdentify.InstanceID);
            }

            ServiceInstaller serviceInstaller = new ServiceInstaller
            {
                Context = new InstallContext(string.Concat(Path.GetTempPath(), "Install_", serviceIdentify.ServiceName, ".log"), new string[] { $"/assemblypath={parameter.AssemblyPath}" }),
                Parent = serviceProcessInstaller,
                ServiceName = serviceIdentify.ServiceName,
                DisplayName = displayName,
                Description = description,
                StartType = parameter.StartType
            };

            if (parameter.ServicesDependedOn != null)
            {
                serviceInstaller.ServicesDependedOn = parameter.ServicesDependedOn.ToArray();
            }

            if (string.IsNullOrEmpty(serviceIdentify.InstanceID) != true)
            {
                if (parameter.Args == null)
                {
                    parameter.Args = new List<string>();
                }

                parameter.Args.Insert(0, $"/InstanceID={serviceIdentify.InstanceID}");
            }

            if (parameter.Args != null)
            {
                serviceInstaller.Context.Parameters["assemblypath"] =
                    $"\"{serviceInstaller.Context.Parameters["assemblypath"]}\" {string.Join(" ", parameter.Args.ToArray())}";
            }

            serviceInstaller.Context.LogMessage($"*** インストールを開始します。{DateTime.Now}");

            ListDictionary state = new ListDictionary();
            serviceInstaller.Install(state);

            SetFailureActions(serviceIdentify.ServiceName, parameter);

            serviceInstaller.Context.LogMessage($"*** インストールが終了しました。{DateTime.Now}");
        }

        public void Uninstall(string serviceBaseName, string instanceID, ServiceInstallParameter parameter)
        {
            if (string.IsNullOrEmpty(serviceBaseName) == true)
            {
                throw new ArgumentException(nameof(serviceBaseName));
            }
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            ServiceIdentify serviceIdentify = new ServiceIdentify()
            {
                ServiceBaseName = serviceBaseName,
                InstanceID = instanceID
            };

            ServiceInstaller serviceInstaller = new ServiceInstaller
            {
                Context = new InstallContext(string.Concat(Path.GetTempPath(), "Uninstall_", serviceIdentify.ServiceName, ".log"), null),
                ServiceName = serviceIdentify.ServiceName
            };

            serviceInstaller.Context.LogMessage($"*** アンインストールを開始します。{DateTime.Now}");

            serviceInstaller.Uninstall(null);

            serviceInstaller.Context.LogMessage($"*** アンインストールが終了しました。{DateTime.Now}");
        }

        protected virtual void SetFailureActions(string serviceName, ServiceInstallParameter parameter)
        {
            if (string.IsNullOrEmpty(serviceName) == true)
            {
                throw new ArgumentException(nameof(serviceName));
            }
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

                ServiceController sc = new ServiceController(serviceName);

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
