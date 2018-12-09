using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm
{
    public static class ServiceControllerExtensions
    {
        public static int GetProcessId(this ServiceController sc)
        {
            if (sc == null)
                throw new ArgumentNullException("sc");

            IntPtr zero = IntPtr.Zero;

            try
            {
                uint dwBytesNeeded;
                // Call once to figure the size of the output buffer.
                try
                {
                    PInvoke.QueryServiceStatusEx(sc.ServiceHandle, PInvoke.SC_STATUS_PROCESS_INFO, zero, 0, out dwBytesNeeded);
                    if (Marshal.GetLastWin32Error() == PInvoke.ERROR_INSUFFICIENT_BUFFER)
                    {
                        // Allocate required buffer and call again.
                        zero = Marshal.AllocHGlobal((int)dwBytesNeeded);

                        if (PInvoke.QueryServiceStatusEx(sc.ServiceHandle, PInvoke.SC_STATUS_PROCESS_INFO, zero, dwBytesNeeded, out dwBytesNeeded))
                        {
                            PInvoke.SERVICE_STATUS_PROCESS ssp = (PInvoke.SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(zero, typeof(PInvoke.SERVICE_STATUS_PROCESS));

                            return (int)ssp.dwProcessId;
                        }
                    }
                }
                catch
                {
                    // sc.ServiceHandle は、権限がない場合に System.InvalidOperationException を引き起こす場合がある
                }
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(zero);
                }
            }
            return -1;
        }

    }
}
