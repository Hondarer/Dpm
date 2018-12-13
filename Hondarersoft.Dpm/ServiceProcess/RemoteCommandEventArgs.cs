using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class RemoteCommandEventArgs : EventArgs
    {
        public int Command { get; internal set; }

        public object Data { get; internal set; }
    }
}
