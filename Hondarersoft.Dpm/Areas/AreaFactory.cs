using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Hondarersoft.Dpm.Areas
{
    public class AreaFactory
    {
        protected const string MMF_HEAD = "Dpm.AreaData_";

        protected static readonly object lockObject = new object();

        protected static AreaFactory instance=null;

        internal readonly Dictionary<string, AreaManageData> manageDataDicionary = new Dictionary<string, AreaManageData>();

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

        public AreaAccessor CreateArea(string name, long capacity, bool isGlobal = true, bool isQueue = false)
        {
            lock (manageDataDicionary)
            {
                if (manageDataDicionary.ContainsKey(name) == true)
                {
                    throw new Exception();
                }

                MemoryMappedFileSecurity customSecurity = new MemoryMappedFileSecurity();
                if (isGlobal == true)
                {
                    customSecurity.AddAccessRule(new AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.ReadWrite, AccessControlType.Allow));
                }

                AreaManageData areaManageData = new AreaManageData
                {
                    SyncMutex = Apis.Sync.CreateMasterMutex(GetAccessDataMutexKey(name), isGlobal),
                    IsCreator = true
                };

                areaManageData.SyncMutex.WaitOne();

                areaManageData.MemoryMappedFile = MemoryMappedFile.CreateOrOpen(GetMmfName(name,isGlobal), Marshal.SizeOf(typeof(AreaFixedHeader)) + Marshal.SizeOf(typeof(AreaVariableHeader)) + capacity, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, customSecurity, HandleInheritability.None);
                areaManageData.HeaderAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(0, Marshal.SizeOf(typeof(AreaFixedHeader)) + Marshal.SizeOf(typeof(AreaVariableHeader)), MemoryMappedFileAccess.ReadWrite);

                areaManageData.AreaFixedHeader = new AreaFixedHeader
                {
                    Blocks = 1,
                    Records = 1,
                    RecordLength = capacity,
                    IsQueue = isQueue
                };

                areaManageData.HeaderAccessor.Write(0, ref areaManageData.AreaFixedHeader);

                AreaVariableHeader areaVariableHeader = new AreaVariableHeader()
                {
                    IsFreezed = false,
                    LastUpdated = DateTime.UtcNow,
                    ReadPointer = 0,
                    WritePointer = 0
                };

                areaManageData.HeaderAccessor.Write(Marshal.SizeOf(typeof(AreaFixedHeader)), ref areaVariableHeader);

                MemoryMappedViewAccessor dataMemoryMappedViewAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(Marshal.SizeOf(typeof(AreaFixedHeader)) + Marshal.SizeOf(typeof(AreaVariableHeader)), capacity, MemoryMappedFileAccess.ReadWrite);

                areaManageData.DataAccessor = new AreaAccessor(dataMemoryMappedViewAccessor, areaManageData);

                manageDataDicionary.Add(name, areaManageData);

                areaManageData.SyncMutex.ReleaseMutex();

                return areaManageData.DataAccessor;
            }
        }

        public AreaAccessor OpenArea(string name, bool isReadOnly = true, bool isGlobal = true)
        {
            lock (manageDataDicionary)
            {
                if (manageDataDicionary.ContainsKey(name) == true)
                {
                    throw new Exception();
                }

                string mmfName = GetMmfName(name, isGlobal);

                if (Apis.Areas.MemoryMappedFileExists(mmfName) != true)
                {
                    throw new IOException("MemoryMappedFile not exists.");
                }

                AreaManageData areaManageData = new AreaManageData
                {
                    MemoryMappedFile = MemoryMappedFile.OpenExisting(mmfName, MemoryMappedFileRights.ReadWrite)
                };

                areaManageData.HeaderAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(0, Marshal.SizeOf(typeof(AreaFixedHeader)) + Marshal.SizeOf(typeof(AreaVariableHeader)), MemoryMappedFileAccess.ReadWrite);

                areaManageData.HeaderAccessor.Read(0, out areaManageData.AreaFixedHeader);

                //Console.WriteLine($"size: {areaManageData.AreaFixedHeader.Blocks * areaManageData.AreaFixedHeader.Records * areaManageData.AreaFixedHeader.RecordLength}");

                AreaVariableHeader areaVariableHeader;
                areaManageData.HeaderAccessor.Read(Marshal.SizeOf(typeof(AreaFixedHeader)), out areaVariableHeader);

                MemoryMappedFileAccess memoryMappedFileDataAccess = MemoryMappedFileAccess.ReadWrite;
                if ((isReadOnly == true) || (areaVariableHeader.IsFreezed == true))
                {
                    memoryMappedFileDataAccess = MemoryMappedFileAccess.Read;
                }

                if ((areaManageData.AreaFixedHeader.IsQueue == true) || (areaVariableHeader.IsFreezed == false))
                {
                    areaManageData.SyncMutex = Apis.Sync.CreateClientMutex(GetAccessDataMutexKey(name), isGlobal);
                }

                MemoryMappedViewAccessor dataMemoryMappedViewAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(Marshal.SizeOf(typeof(AreaFixedHeader)) + Marshal.SizeOf(typeof(AreaVariableHeader)), areaManageData.AreaFixedHeader.Blocks * areaManageData.AreaFixedHeader.Records * areaManageData.AreaFixedHeader.RecordLength, memoryMappedFileDataAccess);
                areaManageData.DataAccessor = new AreaAccessor(dataMemoryMappedViewAccessor, areaManageData);

                manageDataDicionary.Add(name, areaManageData);

                return areaManageData.DataAccessor;
            }
        }

        public AreaAccessor GetAccessor(string name)
        {
            lock (manageDataDicionary)
            {
                return manageDataDicionary[name].DataAccessor;
            }
        }

        public static string GetAccessDataMutexKey(string name)
        {
            return $"AreaSync_{name}";
        }

        ~AreaFactory()
        {
            foreach (AreaManageData areaManageData in manageDataDicionary.Values)
            {
                areaManageData.MemoryMappedFile.Dispose();
            }
        }
    }
}
