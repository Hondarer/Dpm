using System.Diagnostics;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class ServiceProcess
    {
        public static bool IsServiceSession()
        {
            return Process.GetCurrentProcess().SessionId == 0;
        }
    }
}
