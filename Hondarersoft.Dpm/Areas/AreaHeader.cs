using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    public struct AreaHeader
    {
        public long Blocks { get; set; }

        public long Records { get; set; }

        public long RecordLength { get; set; }

        public long ReadPointer { get; set; }

        public long WritePointer { get; set; }
    }
}
