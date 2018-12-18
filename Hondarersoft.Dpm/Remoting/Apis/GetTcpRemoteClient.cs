using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Remoting
    {
        public static T GetTcpRemoteClient<T>(string hostName, int port, string uri = null)
        {
            if (string.IsNullOrWhiteSpace(hostName) == true)
            {
                hostName = "localhost";
            }
            if (uri == null)
            {
                uri = typeof(T).Name;
            }
            return (T)Activator.GetObject(typeof(T), $"tcp://{hostName}:{port}/{uri}");
        }
    }
}
