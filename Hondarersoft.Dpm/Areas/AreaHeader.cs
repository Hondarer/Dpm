using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AreaFixedHeader
    {
        // 将来的に、この値が変更されたら、データを動的に差し替えたい。
        public long Version { get; set; }

        public long RecordLength { get; set; }

        public long Blocks { get; set; }

        public long Records { get; set; }

        public bool IsQueue { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AreaVariableHeader
    {
        public bool IsFreezed { get; set; }

        public DateTime LastUpdated { get; set; }

        public long AvailableBlocks { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AreaBlockHeader
    {
        public long AvailableRecords { get; set; }

        public DateTime LastUpdated { get; set; }

        public long ReadPointer { get; set; }

        public long WritePointer { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AreaRecordHeader
    {
        public DateTime LastUpdated { get; set; }
    }
}
