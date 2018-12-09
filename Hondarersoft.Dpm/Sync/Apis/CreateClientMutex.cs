using System.IO;
using System.Threading;

namespace Hondarersoft.Dpm.Apis
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
                    // 所有者がいなかった

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
