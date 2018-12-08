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

        internal const int ERROR_INSUFFICIENT_BUFFER = 0x7a;
        internal const int SC_STATUS_PROCESS_INFO = 0;

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool QueryServiceStatusEx(SafeHandle hService, int infoLevel, IntPtr lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

    }
}
