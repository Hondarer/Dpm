using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Remoting
    {
        public static T GetIpcRemoteClient<T>(string channelName, string uri = null)
        {
            if (uri == null)
            {
                uri = typeof(T).Name;
            }
            return (T)Activator.GetObject(typeof(T), $"ipc://{channelName}/{uri}");
        }
    }
}
