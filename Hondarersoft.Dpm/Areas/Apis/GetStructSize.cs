using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Areas
    {
        public static int GetStructSize(string assemblyFile, string name)
        {
            Assembly assm = Assembly.LoadFrom(assemblyFile); // e.g. "Hondarersoft.Dpm.dll"

            Type type = assm.GetType(name); // e.g. "Hondarersoft.Dpm.PInvoke+SERVICE_STATUS_PROCESS"

            return GetStructSize(type);
        }

        public static int GetStructSize(Type type)
        {
            return Marshal.SizeOf(type);
        }
    }
}
