using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Remoting
    {
        public static void InitializeIpcClientChannel()
        {
            IpcClientChannel clientChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(clientChannel, true);
        }
    }
}
