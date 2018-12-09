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

        protected static string GetMmfName(string name,bool isGlobal)
        {
            string mmfName = MMF_HEAD;

            if (isGlobal == true)
            {
                mmfName = string.Concat(@"Global\", mmfName);
            }

            mmfName = string.Concat(mmfName, name);

            return mmfName;
        }

        public AreaAccessor CreateArea(string name, long blocks, long records, long recordlength, bool isGlobal = true, bool isQueue = false)
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

                areaManageData.AreaFixedHeader = new AreaFixedHeader
                {
                    Blocks = blocks,
                    Records = records,
                    RecordLength = recordlength,
                    IsQueue = isQueue
                };

                areaManageData.MemoryMappedFile = MemoryMappedFile.CreateOrOpen(GetMmfName(name,isGlobal), areaManageData.GetAreaTotalLength(), MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, customSecurity, HandleInheritability.None);
                areaManageData.HeaderAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(0, areaManageData.GetHeaderTotalLength(), MemoryMappedFileAccess.ReadWrite);

                areaManageData.HeaderAccessor.Write(0, ref areaManageData.AreaFixedHeader);

                DateTime lastUpdated = DateTime.UtcNow;

                AreaVariableHeader areaVariableHeader = new AreaVariableHeader()
                {
                    AvailableBlocks= areaManageData.AreaFixedHeader.Blocks,
                    IsFreezed = false,
                    LastUpdated = lastUpdated
                };

                areaManageData.HeaderAccessor.Write(areaManageData.GetVariableHeaderOffset(), ref areaVariableHeader);

                for (long block = 1; block <= areaManageData.AreaFixedHeader.Blocks; block++)
                {
                    AreaBlockHeader areaBlockHeader = new AreaBlockHeader()
                    {
                        AvailableRecords = areaManageData.AreaFixedHeader.Records,
                        ReadPointer = 0,
                        WritePointer = 0,
                        LastUpdated = lastUpdated
                    };

                    areaManageData.HeaderAccessor.Write(areaManageData.GetBlockHeaderOffset(block), ref areaBlockHeader);

                    for (long record = 1; record <= areaManageData.AreaFixedHeader.Records; record++)
                    {
                        AreaRecordHeader areaRecordHeader = new AreaRecordHeader()
                        {
                            LastUpdated = lastUpdated
                        };

                        areaManageData.HeaderAccessor.Write(areaManageData.GetRecordHeaderOffset(block,record), ref areaRecordHeader);
                    }
                }

                MemoryMappedViewAccessor dataMemoryMappedViewAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(areaManageData.GetHeaderTotalLength(), areaManageData.GetDataTotalLength(), MemoryMappedFileAccess.ReadWrite);

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

                // AreaFixedHeader のみを対象としていったん開く
                areaManageData.HeaderAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(0, Marshal.SizeOf(typeof(AreaFixedHeader)), MemoryMappedFileAccess.ReadWrite);
                // パラメータを得る
                areaManageData.HeaderAccessor.Read(0, out areaManageData.AreaFixedHeader);

                // 容量をパラメータから得たので、アクセサーを開きなおす
                areaManageData.HeaderAccessor.Dispose();
                areaManageData.HeaderAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(0, areaManageData.GetHeaderTotalLength(), MemoryMappedFileAccess.ReadWrite);

                //Console.WriteLine($"size: {areaManageData.AreaFixedHeader.Blocks * areaManageData.AreaFixedHeader.Records * areaManageData.AreaFixedHeader.RecordLength}");

                AreaVariableHeader areaVariableHeader;
                areaManageData.HeaderAccessor.Read(areaManageData.GetVariableHeaderOffset(), out areaVariableHeader);

                MemoryMappedFileAccess memoryMappedFileDataAccess = MemoryMappedFileAccess.ReadWrite;
                if ((isReadOnly == true) || (areaVariableHeader.IsFreezed == true))
                {
                    memoryMappedFileDataAccess = MemoryMappedFileAccess.Read;
                }

                if ((areaManageData.AreaFixedHeader.IsQueue == true) || (areaVariableHeader.IsFreezed == false))
                {
                    areaManageData.SyncMutex = Apis.Sync.CreateClientMutex(GetAccessDataMutexKey(name), isGlobal);
                }

                MemoryMappedViewAccessor dataMemoryMappedViewAccessor = areaManageData.MemoryMappedFile.CreateViewAccessor(areaManageData.GetHeaderTotalLength(), areaManageData.GetDataTotalLength(), memoryMappedFileDataAccess);
                areaManageData.DataAccessor = new AreaAccessor(dataMemoryMappedViewAccessor, areaManageData);

                manageDataDicionary.Add(name, areaManageData);

                return areaManageData.DataAccessor;
            }
        }

        public static bool IsAreaExists(string name, bool isGlobal = true)
        {
            string mmfName = GetMmfName(name, isGlobal);

            if (Apis.Areas.MemoryMappedFileExists(mmfName) == true)
            {
                return true;
            }

            return false;
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
