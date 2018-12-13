using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    internal class AreaManageData
    {
        public MemoryMappedFile MemoryMappedFile { get; set; }
        public MemoryMappedViewAccessor HeaderAccessor { get; set; }
        public AreaAccessor DataAccessor { get; set; }
        public Mutex SyncMutex { get; set; }
        public AreaFixedHeader AreaFixedHeader;

        public bool IsCreator { get; set; }

        public long GetHeaderTotalLength()
        {
            return Marshal.SizeOf(typeof(AreaFixedHeader)) +
                Marshal.SizeOf(typeof(AreaVariableHeader)) +
                Marshal.SizeOf(typeof(AreaBlockHeader)) * AreaFixedHeader.Blocks +
                Marshal.SizeOf(typeof(AreaRecordHeader)) * AreaFixedHeader.Blocks * AreaFixedHeader.Records;
        }

        public long GetDataTotalLength()
        {
            return AreaFixedHeader.Blocks * AreaFixedHeader.Records * AreaFixedHeader.RecordLength;
        }

        public long GetAreaTotalLength()
        {
            return GetHeaderTotalLength() + GetDataTotalLength();
        }

        public long GetVariableHeaderOffset()
        {
            return Marshal.SizeOf(typeof(AreaFixedHeader));
        }

        public long GetBlockHeaderOffset(long block)
        {
            return Marshal.SizeOf(typeof(AreaFixedHeader)) +
                Marshal.SizeOf(typeof(AreaVariableHeader)) +
                Marshal.SizeOf(typeof(AreaBlockHeader)) * (block - 1);
        }

        public long GetRecordHeaderOffset(long block, long record)
        {
            return Marshal.SizeOf(typeof(AreaFixedHeader)) +
                Marshal.SizeOf(typeof(AreaVariableHeader)) +
                Marshal.SizeOf(typeof(AreaBlockHeader)) * AreaFixedHeader.Blocks +
                Marshal.SizeOf(typeof(AreaRecordHeader)) * (((block - 1) * AreaFixedHeader.Records) + (record - 1));
        }
    }
}
