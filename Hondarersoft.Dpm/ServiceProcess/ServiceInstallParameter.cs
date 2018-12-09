using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class ServiceInstallParameter
    {
        public enum FAILURE_ACTION_TYPE : int
        {
            FAILURE_ACTION_NONE = PInvoke.SC_ACTION_TYPE.SC_ACTION_NONE,
            FAILURE_ACTION_REBOOT = PInvoke.SC_ACTION_TYPE.SC_ACTION_REBOOT,
            FAILURE_ACTION_RESTART = PInvoke.SC_ACTION_TYPE.SC_ACTION_RESTART,
            FAILURE_ACTION_RUN_COMMAND = PInvoke.SC_ACTION_TYPE.SC_ACTION_RUN_COMMAND
        }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public ServiceAccount Account { get; set; } = ServiceAccount.LocalSystem;

        public ServiceStartMode StartType { get; set; } = ServiceStartMode.Manual;

        private string assemblyPath;

        public string AssemblyPath
        {
            get
            {
                if(string.IsNullOrEmpty(assemblyPath)==true)
                {
                    return Assembly.GetEntryAssembly().Location;
                }

                return assemblyPath;
            }
            set
            {
                assemblyPath = value;
            }
        }

        public List<string> Args { get; set; }

        public List<string> ServicesDependedOn { get; set; }

        public FAILURE_ACTION_TYPE FirstFailureActionType { get; set; }
        public FAILURE_ACTION_TYPE SecondFailureActionType { get; set; }
        public FAILURE_ACTION_TYPE SubsequentFailureActionType { get; set; }

        public TimeSpan FirstFailureActionDelay { get; set; } = TimeSpan.Zero;
        public TimeSpan SecondFailureActionDelay { get; set; } = TimeSpan.Zero;
        public TimeSpan SubsequentFailureActionDelay { get; set; } = TimeSpan.Zero;

        public uint DaysToResetFailureCount { get; set; }

        public string CommandlineOfFailure { get; set; }

        public string RebootMessageOfFailure { get; set; }

        public List<string> ExecutableUsers { get; set; }
    }
}
