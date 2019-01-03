using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Remoting
    {
        public static T GetIpcRemoteClient<T>(Type remoteServiceType, string instanceID = null)
        {
            string channelName = remoteServiceType.Name;

            if (string.IsNullOrWhiteSpace(instanceID) != true)
            {
                channelName += $"_{instanceID}";
            }

            return GetIpcRemoteClient<T>(channelName);
        }

        public static T GetIpcRemoteClient<T>(string channelName)
        {
            return (T)Activator.GetObject(typeof(T), $"ipc://{channelName}/{typeof(T).Name}");
        }
    }
}
