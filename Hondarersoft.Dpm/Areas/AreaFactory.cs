using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    public class AreaFactory
    {
        protected const string MMF_HEAD = "Dpm.";

        protected static readonly object lockObject = new object();

        protected static AreaFactory instance=null;

        protected Dictionary<string, MemoryMappedFile> openingMmf = new Dictionary<string, MemoryMappedFile>();

        public static AreaFactory Instance
        {
            get
            {
                if(instance==null)
                {
                    lock(lockObject)
                    {
                        if(instance==null)
                        {
                            instance = new AreaFactory();
                        }
                    }
                }

                return instance;
            }
        }

        protected string GetMmfName(string name,bool isGlobal)
        {
            string mmfName = MMF_HEAD;

            if (isGlobal == true)
            {
                mmfName = string.Concat(@"Global\", mmfName);
            }

            mmfName = string.Concat(mmfName, name);

            return mmfName;
        }

        public void CreateArea(string name, long capacity, bool othersReadonly = false, bool isGlobal = true)
        {
            lock (openingMmf)
            {
                if (openingMmf.ContainsKey(name) == true)
                {
                    throw new Exception();
                }

                MemoryMappedFileSecurity customSecurity = new MemoryMappedFileSecurity();
                if (isGlobal == true)
                {
                    customSecurity.AddAccessRule(new AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.ReadWrite, AccessControlType.Allow));
                }

                MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(GetMmfName(name,isGlobal), capacity, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, customSecurity, HandleInheritability.None);

                openingMmf.Add(name, mmf);

                //MemoryMappedViewAccessor accesser = mmf.CreateViewAccessor();

            }
        }

        public void OpenArea(string name, bool isGlobal = true)
        {
            lock (openingMmf)
            {
                if (openingMmf.ContainsKey(name) == true)
                {
                    throw new Exception();
                }

                string mmfName = GetMmfName(name, isGlobal);

                if (Apis.Areas.MemoryMappedFileExists(mmfName) != true)
                {
                    Console.WriteLine("MemoryMappedFile not exists.");
                    throw new Exception();
                }

                MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mmfName, MemoryMappedFileRights.ReadWrite);

                openingMmf.Add(name, mmf);

                //#if false
                //                // 読み取り専用で開く(アクセサーの書き込み操作は例外となる)
                //                MemoryMappedFile _mmf = MemoryMappedFile.OpenExisting(@"Global\Hondarersoft.Dpm.TestMemory", MemoryMappedFileRights.Read);
                //                MemoryMappedViewAccessor _accesser = _mmf.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.Read);
                //#else
                //                // 読み書き可能で開く
                //                MemoryMappedFile _mmf = MemoryMappedFile.OpenExisting(@"Global\Hondarersoft.Dpm.TestMemory", MemoryMappedFileRights.ReadWrite);
                //                MemoryMappedViewAccessor _accesser = _mmf.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.ReadWrite);
                //#endif
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
