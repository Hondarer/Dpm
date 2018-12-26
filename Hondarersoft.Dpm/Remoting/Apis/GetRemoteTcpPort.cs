using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Configuration;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Remoting
    {
        public static int GetRemoteTcpPort(DpmServiceBase serviceBase, RemoteCommandService remoteObject)
        {
            if (serviceBase == null)
            {
                throw new ArgumentNullException(nameof(serviceBase));
            }

            if (remoteObject == null)
            {
                throw new ArgumentNullException(nameof(remoteObject));
            }

            string instanceID = null;
            if (serviceBase.SupportMultiInstance == true)
            {
                instanceID = serviceBase.InstanceID;
            }

            return GetRemoteTcpPort(serviceBase.GetType(), remoteObject.GetType(), instanceID);
        }

        public static int GetRemoteTcpPort(Type remoteServiceType, Type remoteObjectType, string instanceID = null)
        {
            if (remoteObjectType == null)
            {
                throw new ArgumentNullException(nameof(remoteObjectType));
            }

            string settingKey = string.Empty;

            if (remoteServiceType != null)
            {
                if (string.IsNullOrWhiteSpace(instanceID) != true)
                {
                    settingKey = $"{remoteServiceType.Name}_{instanceID}.";
                }
                else
                {
                    settingKey = $"{remoteServiceType.Name}.";
                }
            }

            settingKey += $"{remoteObjectType.Name}.TcpPort";

            string portString = ConfigurationManager.AppSettings.Get(settingKey);

            if (portString != null)
            {
                if (int.TryParse(portString, out int port) == true)
                {
                    return port;
                }
            }

            return 0;
        }
    }
}
