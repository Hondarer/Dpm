using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Reflection;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class ServiceInstallParameter : ServiceIdentify
    {
        private string displayName;

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName) == true)
                {
                    if (string.IsNullOrEmpty(InstanceID) == true)
                    {
                        return ServiceBaseName;
                    }

                    return string.Concat(ServiceBaseName, " - ", InstanceID);
                }

                if (string.IsNullOrEmpty(InstanceID) == true)
                {
                    return displayName;
                }

                return string.Concat(displayName, " - ", InstanceID);
            }
            set
            {
                displayName = value;
            }
        }

        private string description;

        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(description) == true)
                {
                    if (string.IsNullOrEmpty(InstanceID) == true)
                    {
                        return ServiceBaseName;
                    }

                    return string.Concat(ServiceBaseName, " - ", InstanceID);
                }

                if (string.IsNullOrEmpty(InstanceID) == true)
                {
                    return description;
                }

                return string.Concat(description, " - ", InstanceID);
            }
            set
            {
                description = value;
            }
        }

        public ServiceAccount Account { get; set; } = ServiceAccount.LocalSystem;

        public ServiceStartMode StartType { get; set; } = ServiceStartMode.Manual;

        private string assemblyPath;

        public string AssemblyPath
        {
            get
            {
                if(string.IsNullOrEmpty(assemblyPath)==true)
                {
                    return Assembly.GetEntryAssembly().Location;
                }

                return assemblyPath;
            }
            set
            {
                assemblyPath = value;
            }
        }

        public List<string> Args { get; set; }

        public List<string> ServicesDependedOn { get; set; }
    }
}
