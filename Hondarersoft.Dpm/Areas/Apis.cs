using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    public static partial class Apis
    {
        public static bool MemoryMappedFileExists(string mapName)
        {
            bool exists = false;
            IntPtr hFileMapping;

            hFileMapping = PInvoke.OpenFileMapping(PInvoke.FILE_MAP_READ, false, mapName);

            if (hFileMapping != IntPtr.Zero)
            {
                exists = true;
                PInvoke.CloseHandle(hFileMapping);
            }

            return exists;
        }

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
