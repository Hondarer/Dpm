using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public static partial class Apis
    {
        public static bool IsServiceExists(string name)
        {
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController sc in services)
            {
                if (sc.ServiceName == name)
                {
                    return true;
                }
            }

            return false;
        }

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

        public static bool StartService(string name)
        {
            if (IsServiceExists(name) == true)
            {
                ServiceController sc = new ServiceController(name);
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    // NOP
                }
                else
                {
                    try
                    {
                        sc.Start();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{name} could not be started.\r\n{ex}");
                    }
                }
            }

            return false;
        }

        public static bool StopService(string name)
        {
            if (IsServiceExists(name) == true)
            {
                ServiceController sc = new ServiceController(name);
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    // NOP
                }
                else
                {
                    try
                    {
                        sc.Stop();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{name} could not be stopped.\r\n{ex}");
                    }
                }
            }

            return false;
        }
    }
}
