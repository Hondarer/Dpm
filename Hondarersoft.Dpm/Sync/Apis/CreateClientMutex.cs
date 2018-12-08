using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Sync.Apis
{
    public static partial class Sync
    {
        public static Mutex CreateClientMutex(string name, bool isGlobal = true)
        {
            Mutex mutex;
            string mutexName = GetMutexName(name, isGlobal);

            try
            {
                bool createNew;
                mutex = new Mutex(false, mutexName, out createNew);

                if (createNew != false)
                {
                    mutex.Close();
                    mutex.Dispose();
                    throw new IOException($"A mutex named '{mutexName}' is not exist.");
                }
            }
            catch
            {
                // 取得失敗
                throw;
            }

            return mutex;
        }
    }
}
