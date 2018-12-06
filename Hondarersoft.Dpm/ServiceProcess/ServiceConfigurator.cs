using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class ServiceConfigurator
    {
        private const int SERVICE_ALL_ACCESS = 0xF01FF;
        private const int SC_MANAGER_ALL_ACCESS = 0xF003F;
        private const int SERVICE_CONFIG_DESCRIPTION = 0x1;
        private const int SERVICE_CONFIG_FAILURE_ACTIONS = 0x2;
        private const int SERVICE_NO_CHANGE = -1;
        private const int ERROR_ACCESS_DENIED = 5;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SERVICE_FAILURE_ACTIONS
        {
            public int dwResetPeriod;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpRebootMsg;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpCommand;
            public int cActions;
            public IntPtr lpsaActions;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SERVICE_DESCRIPTION
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDescription;
        }

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        private static extern bool ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS lpInfo);
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        private static extern bool ChangeServiceDescription(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        public IntPtr ServiceHandle { get; protected set; }

        public ServiceConfigurator(ServiceController svcController)
        {
            ServiceHandle = svcController.ServiceHandle.DangerousGetHandle();
        }

        public enum SC_ACTION_TYPE : int
        {
            SC_ACTION_NONE=0,
            SC_ACTION_REBOOT=2,
            SC_ACTION_RESTART=1,
            SC_ACTION_RUN_COMMAND=3
        }


        public class FailureAction
        {
            public SC_ACTION_TYPE Type {get;set;}
            public int Delay { get; set; }
            }

        public void SetRecoveryOptions(FailureAction pFirstFailure, FailureAction pSecondFailure, FailureAction pSubsequentFailures, int pDaysToResetFailureCount = 0)
        {
            int NUM_ACTIONS = 3;
            int[] arrActions = new int[NUM_ACTIONS * 2];
            int index = 0;
            arrActions[index++] = (int)pFirstFailure.Type;
            arrActions[index++] = pFirstFailure.Delay;
            arrActions[index++] = (int)pSecondFailure.Type;
            arrActions[index++] = pSecondFailure.Delay;
            arrActions[index++] = (int)pSubsequentFailures.Type;
            arrActions[index++] = pSubsequentFailures.Delay;

            IntPtr tmpBuff = Marshal.AllocCoTaskMem(NUM_ACTIONS * 8);

            try
            {
                Marshal.Copy(arrActions, 0, tmpBuff, NUM_ACTIONS * 2);
                SERVICE_FAILURE_ACTIONS sfa = new SERVICE_FAILURE_ACTIONS
                {
                    cActions = 3,
                    dwResetPeriod = pDaysToResetFailureCount,
                    lpCommand = null,
                    lpRebootMsg = null,
                    lpsaActions = new IntPtr(tmpBuff.ToInt32())
                };

                bool success = ChangeServiceFailureActions(ServiceHandle, SERVICE_CONFIG_FAILURE_ACTIONS, ref sfa);
                if (!success)
                {
                    if (GetLastError() == ERROR_ACCESS_DENIED)
                        throw new Exception("Access denied while setting failure actions.");
                    else
                        throw new Exception("Unknown error while setting failure actions.");
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(tmpBuff);
                tmpBuff = IntPtr.Zero;
            }
        }
    }
}
