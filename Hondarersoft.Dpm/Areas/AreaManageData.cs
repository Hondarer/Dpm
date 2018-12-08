using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
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
    }
}
