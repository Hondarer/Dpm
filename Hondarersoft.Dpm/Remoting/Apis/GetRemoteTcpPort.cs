using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Configuration;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Remoting
    {
        public static int GetRemoteTcpPort(RemoteCommandService remoteObject)
        {
            return GetRemoteTcpPort(remoteObject.GetType());
        }

        public static int GetRemoteTcpPort(Type remoteObjectType)
        {
            string portString = ConfigurationManager.AppSettings.Get($"{remoteObjectType.Name}.TcpPort");

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
