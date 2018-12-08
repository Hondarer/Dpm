using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    public class AreaAccessor
    {
        protected MemoryMappedViewAccessor MemoryMappedViewAccessor { get; set; }

        protected Mutex Mutex { get; set; }

        public AreaAccessor(MemoryMappedViewAccessor memoryMappedViewAccessor, Mutex mutex)
        {
            MemoryMappedViewAccessor = memoryMappedViewAccessor;
            Mutex = mutex;
        }

        public virtual void Read<T>(out T structure) where T : struct
        {
            if (Mutex != null)
            {
                Mutex.WaitOne();
            }
            // TODO: 今の構造だと、途中で Freeze された際の Mutex 不要チェックができない
            MemoryMappedViewAccessor.Read(0, out structure);
            if (Mutex != null)
            {
                Mutex.ReleaseMutex();
            }
        }

        public virtual void Write<T>(ref T structure) where T : struct
        {
            if (Mutex != null)
            {
                Mutex.WaitOne();
            }
            MemoryMappedViewAccessor.Write(0, ref structure);
            if (Mutex != null)
            {
                Mutex.ReleaseMutex();
            }
        }
    }
}
