using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

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
