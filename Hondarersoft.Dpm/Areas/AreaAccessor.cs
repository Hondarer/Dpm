using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;

namespace Hondarersoft.Dpm.Areas
{
    public class AreaAccessor
    {
        protected MemoryMappedViewAccessor MemoryMappedViewAccessor { get; set; }

        internal AreaManageData AreaManageData { get; set; }

        internal AreaAccessor(MemoryMappedViewAccessor memoryMappedViewAccessor, AreaManageData areaManageData)
        {
            MemoryMappedViewAccessor = memoryMappedViewAccessor;
            AreaManageData = areaManageData;
        }

        public void Freeze()
        {
            if (AreaManageData.AreaFixedHeader.IsQueue == true)
            {
                throw new NotSupportedException("Area is a queue.");
            }
            if (AreaManageData.IsCreator != true)
            {
                throw new InvalidOperationException("This process is not a creator of the area.");
            }

            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.WaitOne();

                AreaVariableHeader areaVariableHeader;
                AreaManageData.HeaderAccessor.Read(Marshal.SizeOf(typeof(AreaFixedHeader)), out areaVariableHeader);
                if (areaVariableHeader.IsFreezed == false)
                {
                    areaVariableHeader.IsFreezed = true;
                    areaVariableHeader.LastUpdated = DateTime.UtcNow;
                    AreaManageData.HeaderAccessor.Write(Marshal.SizeOf(typeof(AreaFixedHeader)), ref areaVariableHeader);

                    MemoryMappedViewAccessor.Dispose();
                    MemoryMappedViewAccessor = AreaManageData.MemoryMappedFile.CreateViewAccessor(Marshal.SizeOf(typeof(AreaFixedHeader)) + Marshal.SizeOf(typeof(AreaVariableHeader)), AreaManageData.AreaFixedHeader.Blocks * AreaManageData.AreaFixedHeader.Records * AreaManageData.AreaFixedHeader.RecordLength, MemoryMappedFileAccess.Read);

                    Mutex _mutex = AreaManageData.SyncMutex;
                    AreaManageData.SyncMutex = null;
                    _mutex.ReleaseMutex();
                }
                else
                {
                    AreaManageData.SyncMutex.ReleaseMutex();
                }
            }
        }

        // TODO: Read 前後、Write 前後の処理を関数に切り出す

        public virtual void Read<T>(out T structure) where T : struct
        {
            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.WaitOne();
            }

            MemoryMappedViewAccessor.Read(0, out structure);

            if (AreaManageData.SyncMutex != null)
            {
                if (AreaManageData.AreaFixedHeader.IsQueue == false)
                {
                    AreaVariableHeader areaVariableHeader;
                    AreaManageData.HeaderAccessor.Read(Marshal.SizeOf(typeof(AreaFixedHeader)), out areaVariableHeader);
                    if (areaVariableHeader.IsFreezed == true)
                    {
                        MemoryMappedViewAccessor.Dispose();
                        MemoryMappedViewAccessor = AreaManageData.MemoryMappedFile.CreateViewAccessor(Marshal.SizeOf(typeof(AreaFixedHeader)) + Marshal.SizeOf(typeof(AreaVariableHeader)), AreaManageData.AreaFixedHeader.Blocks * AreaManageData.AreaFixedHeader.Records * AreaManageData.AreaFixedHeader.RecordLength, MemoryMappedFileAccess.Read);

                        Mutex _mutex = AreaManageData.SyncMutex;
                        AreaManageData.SyncMutex = null;
                        _mutex.ReleaseMutex();
                    }
                    else
                    {
                        AreaManageData.SyncMutex.ReleaseMutex();
                    }
                }
                else
                {
                    AreaManageData.SyncMutex.ReleaseMutex();
                }
            }
        }

        public virtual void Write<T>(ref T structure) where T : struct
        {
            if (MemoryMappedViewAccessor.CanWrite == false)
            {
                throw new NotSupportedException();
            }

            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.WaitOne();
            }

            MemoryMappedViewAccessor.Write(0, ref structure);

            AreaVariableHeader areaVariableHeader;
            AreaManageData.HeaderAccessor.Read(Marshal.SizeOf(typeof(AreaFixedHeader)), out areaVariableHeader);
            areaVariableHeader.LastUpdated = DateTime.UtcNow;
            AreaManageData.HeaderAccessor.Write(Marshal.SizeOf(typeof(AreaFixedHeader)), ref areaVariableHeader);

            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.ReleaseMutex();
            }
        }
    }
}
