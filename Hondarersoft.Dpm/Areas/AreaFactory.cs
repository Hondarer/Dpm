using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    public class AreaFactory
    {
        protected Dictionary<string, MemoryMappedFile> openingMmf = new Dictionary<string, MemoryMappedFile>();

        public void CreateArea(string name, long capacity, bool isGlobal = true)
        {
            lock (openingMmf)
            {
                if (openingMmf.ContainsKey(name) == true)
                {
                    throw new Exception();
                }

                string mmfName = "Dpm.";

                MemoryMappedFileSecurity customSecurity = new MemoryMappedFileSecurity();
                if (isGlobal == true)
                {
                    mmfName = string.Concat(@"Global\", mmfName);
                    customSecurity.AddAccessRule(new AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.ReadWrite, AccessControlType.Allow));
                }

                mmfName = string.Concat(mmfName, name);

                MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(mmfName, capacity, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, customSecurity, HandleInheritability.None);

                openingMmf.Add(name, mmf);

                MemoryMappedViewAccessor accesser = mmf.CreateViewAccessor();

            }
        }

        public MemoryMappedViewAccessor GetAccessor(string name)
        {
            lock (openingMmf)
            {
                if (openingMmf.ContainsKey(name) != true)
                {
                    throw new Exception();
                }

                return openingMmf[name].CreateViewAccessor();
            }
        }

        ~AreaFactory()
        {
            foreach (MemoryMappedFile mmf in openingMmf.Values)
            {
                mmf.Dispose();
            }
        }
    }
}
