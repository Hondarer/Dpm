using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading.Tasks;

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
