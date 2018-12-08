using System.ServiceProcess;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class ServiceProcess
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
    }
}
