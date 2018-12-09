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
    }
}
