using System;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Remoting
    {
        public static T GetTcpRemoteClient<T>(Type remoteServiceType, string hostName = null, string instanceID = null)
        {
            if (string.IsNullOrWhiteSpace(hostName) == true)
            {
                hostName = "localhost";
            }

            int port = GetRemoteTcpPort(remoteServiceType, typeof(T), instanceID);

            return (T)Activator.GetObject(typeof(T), $"tcp://{hostName}:{port}/{typeof(T).Name}");
        }
    }
}
