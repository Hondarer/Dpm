using System.Diagnostics;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class ServiceProcess
    {
        public static ServiceController GetMyServiceController()
        {
            int myPid = Process.GetCurrentProcess().Id;

            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController sc in services)
            {
                if (sc.GetProcessId() == myPid)
                {
                    return sc;
                }
            }

            return null;
        }
    }
}
