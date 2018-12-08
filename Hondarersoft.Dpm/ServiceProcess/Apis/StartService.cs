using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class ServiceProcess
    {
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

    }
}
