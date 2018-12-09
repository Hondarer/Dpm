﻿using System;
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

            if (MemoryMappedViewAccessor.CanWrite != true)
            {
                throw new InvalidOperationException("Area is readonly or freezed.");
            }

            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.WaitOne();

                AreaVariableHeader areaVariableHeader;
                AreaManageData.HeaderAccessor.Read(AreaManageData.GetVariableHeaderOffset(), out areaVariableHeader);
                if (areaVariableHeader.IsFreezed == false)
                {
                    areaVariableHeader.IsFreezed = true;
                    areaVariableHeader.LastUpdated = DateTime.UtcNow;
                    AreaManageData.HeaderAccessor.Write(AreaManageData.GetVariableHeaderOffset(), ref areaVariableHeader);

                    MemoryMappedViewAccessor.Dispose();
                    MemoryMappedViewAccessor = AreaManageData.MemoryMappedFile.CreateViewAccessor(AreaManageData.GetHeaderTotalLength(), AreaManageData.GetDataTotalLength(), MemoryMappedFileAccess.Read);

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

        public bool IsFreezed()
        {
            bool isFreezed = false;

            if (AreaManageData.AreaFixedHeader.IsQueue == true)
            {
                throw new NotSupportedException("Area is a queue.");
            }

            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.WaitOne();

                AreaVariableHeader areaVariableHeader;
                AreaManageData.HeaderAccessor.Read(AreaManageData.GetVariableHeaderOffset(), out areaVariableHeader);
                if (areaVariableHeader.IsFreezed == true)
                {
                    if (MemoryMappedViewAccessor.CanWrite == true)
                    {
                        MemoryMappedViewAccessor.Dispose();
                        MemoryMappedViewAccessor = AreaManageData.MemoryMappedFile.CreateViewAccessor(AreaManageData.GetHeaderTotalLength(), AreaManageData.GetDataTotalLength(), MemoryMappedFileAccess.Read);
                    }

                    Mutex _mutex = AreaManageData.SyncMutex;
                    AreaManageData.SyncMutex = null;
                    _mutex.ReleaseMutex();

                    isFreezed = true;
                }
                else
                {
                    AreaManageData.SyncMutex.ReleaseMutex();
                }
            }
            else
            {
                // AreaManageData.SyncMutex が null のときは、Freezed
                isFreezed = true;
            }

            return isFreezed;
        }


        protected virtual void OnPreviewRead()
        {
            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.WaitOne();

                AreaVariableHeader areaVariableHeader;
                AreaManageData.HeaderAccessor.Read(AreaManageData.GetVariableHeaderOffset(), out areaVariableHeader);
                if (areaVariableHeader.IsFreezed == true)
                {
                    if (MemoryMappedViewAccessor.CanWrite == true)
                    {
                        MemoryMappedViewAccessor.Dispose();
                        MemoryMappedViewAccessor = AreaManageData.MemoryMappedFile.CreateViewAccessor(AreaManageData.GetHeaderTotalLength(), AreaManageData.GetDataTotalLength(), MemoryMappedFileAccess.Read);
                    }

                    Mutex _mutex = AreaManageData.SyncMutex;
                    AreaManageData.SyncMutex = null;
                    _mutex.ReleaseMutex();
                }
            }
        }

        protected virtual void OnAfterRead()
        {
            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.ReleaseMutex();
            }
        }

        protected virtual void OnPreviewWrite(out DateTime lastUpdated, out AreaVariableHeader areaVariableHeader)
        {
            if (MemoryMappedViewAccessor.CanWrite == false)
            {
                throw new InvalidOperationException("Area is readonly or freezed.");
            }

            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.WaitOne();
            }

            AreaManageData.HeaderAccessor.Read(AreaManageData.GetVariableHeaderOffset(), out areaVariableHeader);
            if (areaVariableHeader.IsFreezed == true)
            {
                MemoryMappedViewAccessor.Dispose();
                MemoryMappedViewAccessor = AreaManageData.MemoryMappedFile.CreateViewAccessor(AreaManageData.GetHeaderTotalLength(), AreaManageData.GetDataTotalLength(), MemoryMappedFileAccess.Read);

                Mutex _mutex = AreaManageData.SyncMutex;
                AreaManageData.SyncMutex = null;
                _mutex.ReleaseMutex();

                throw new InvalidOperationException("Area is freezed.");
            }

            lastUpdated = DateTime.UtcNow;
        }

        protected virtual void OnBlockWrite(ref DateTime lastUpdated, long block)
        {
            AreaManageData.HeaderAccessor.Read(AreaManageData.GetBlockHeaderOffset(block), out AreaRecordHeader areaBlockHeader);
            areaBlockHeader.LastUpdated = lastUpdated;
            AreaManageData.HeaderAccessor.Write(AreaManageData.GetBlockHeaderOffset(block), ref areaBlockHeader);
        }

        protected virtual void OnRecordWrite(ref DateTime lastUpdated,long block,long record)
        {
            AreaManageData.HeaderAccessor.Read(AreaManageData.GetRecordHeaderOffset(block, record), out AreaRecordHeader areaRecordHeader);
            areaRecordHeader.LastUpdated = lastUpdated;
            AreaManageData.HeaderAccessor.Write(AreaManageData.GetRecordHeaderOffset(block, record), ref areaRecordHeader);
        }

        protected virtual void OnAfterWrite(ref DateTime lastUpdated, ref AreaVariableHeader areaVariableHeader)
        {
            areaVariableHeader.LastUpdated = lastUpdated;
            AreaManageData.HeaderAccessor.Write(AreaManageData.GetVariableHeaderOffset(), ref areaVariableHeader);

            if (AreaManageData.SyncMutex != null)
            {
                AreaManageData.SyncMutex.ReleaseMutex();
            }
        }

        public virtual void Read<T>(out T structure) where T : struct
        {
            OnPreviewRead();

            try
            {
                MemoryMappedViewAccessor.Read(0, out structure);
            }
            finally
            {
                OnAfterRead();
            }
        }

        public virtual void Write<T>(ref T structure) where T : struct
        {
            OnPreviewWrite(out DateTime lastUpdated, out AreaVariableHeader areaVariableHeader);

            try
            {
                MemoryMappedViewAccessor.Write(0, ref structure);
                OnRecordWrite(ref lastUpdated, 1, 1);
                OnBlockWrite(ref lastUpdated, 1);
            }
            finally
            {
                OnAfterWrite(ref lastUpdated, ref areaVariableHeader);
            }
        }
    }
}
