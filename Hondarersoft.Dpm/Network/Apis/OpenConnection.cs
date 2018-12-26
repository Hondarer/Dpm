using static Hondarersoft.Dpm.PInvoke;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Network
    {
        public static void OpenConnection(string path, string username, string password)
        {
            NETRESOURCE netResource = new NETRESOURCE
            {
                dwScope = 0,
                dwType = 1,
                dwDisplayType = 0,
                dwUsage = 0,
                lpLocalName = "",
                lpRemoteName = path,
                lpProvider = ""
            };

            WNetCancelConnection2(path, 0, true);
            WNetAddConnection2(ref netResource, password, username, 0);
        }
    }
}
