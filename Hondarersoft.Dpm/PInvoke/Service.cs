using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class PInvoke
    {
        internal const int ERROR_INSUFFICIENT_BUFFER = 0x7a;
        internal const int SC_STATUS_PROCESS_INFO = 0;
        internal const int SERVICE_ALL_ACCESS = 0xF01FF;
        internal const int SC_MANAGER_ALL_ACCESS = 0xF003F;
        internal const int SERVICE_CONFIG_DESCRIPTION = 0x1;
        internal const int SERVICE_CONFIG_FAILURE_ACTIONS = 0x2;
        internal const int SERVICE_NO_CHANGE = -1;
        internal const int ERROR_ACCESS_DENIED = 5;

        internal enum SC_ACTION_TYPE : int
        {
            SC_ACTION_NONE = 0,
            SC_ACTION_REBOOT = 2,
            SC_ACTION_RESTART = 1,
            SC_ACTION_RUN_COMMAND = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SERVICE_STATUS_PROCESS
        {
            public uint dwServiceType;
            public uint dwCurrentState;
            public uint dwControlsAccepted;
            public uint dwWin32ExitCode;
            public uint dwServiceSpecificExitCode;
            public uint dwCheckPoint;
            public uint dwWaitHint;
            public uint dwProcessId;
            public uint dwServiceFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SERVICE_FAILURE_ACTIONS
        {
            public uint dwResetPeriod;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpRebootMsg;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpCommand;
            public uint cActions;
            public IntPtr lpsaActions;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SERVICE_DESCRIPTION
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDescription;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool QueryServiceStatusEx(SafeHandle hService, int infoLevel, IntPtr lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2", SetLastError = true)]
        internal static extern bool ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel, ref SERVICE_FAILURE_ACTIONS lpInfo);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2", SetLastError = true)]
        internal static extern bool ChangeServiceDescription(IntPtr hService, int dwInfoLevel, ref SERVICE_DESCRIPTION lpInfo);
    }
}
