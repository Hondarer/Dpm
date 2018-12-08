using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Areas
    {
        public static int GetStructSize(string assemblyFile, string name)
        {
            Assembly assm = Assembly.LoadFrom(assemblyFile); // e.g. "Hondarersoft.dpm.dll"

            Type type = assm.GetType(name); // e.g. "Hondarersoft.Dpm.PInvoke+SERVICE_STATUS_PROCESS"

            return GetStructSize(type);
        }

        public static int GetStructSize(Type type)
        {
            return Marshal.SizeOf(type);
        }
    }
}
