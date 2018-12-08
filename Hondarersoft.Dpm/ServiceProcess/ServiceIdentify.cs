using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class ServiceIdentify
    {
        public string ServiceBaseName { get; set; }
        public string InstanceID { get; set; }

        public string ServiceName
        {
            get
            {
                if (string.IsNullOrEmpty(InstanceID) == true)
                {
                    return ServiceBaseName;
                }

                return string.Concat(ServiceBaseName, "_", InstanceID);
            }
        }

    }
}
