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
