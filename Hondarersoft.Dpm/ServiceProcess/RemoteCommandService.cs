using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class RemoteCommandService : MarshalByRefObject
    {
        internal delegate object RemoteCommandEventHandler(object sender, RemoteCommandEventArgs eventArgs);
        internal event RemoteCommandEventHandler OnRemoteCommand;

        public virtual object RemoteCommand(int command, object data)
        {
            if (OnRemoteCommand != null)
            {
                return OnRemoteCommand(this, new RemoteCommandEventArgs() { Command = command, Data = data });
            }

            return null;
        }

        public override object InitializeLifetimeService()
        {
            // オブジェクトのリース期限を無期限にする
            return null;
        }
    }
}
