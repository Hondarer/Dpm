using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Sync.Apis
{
    public static partial class Sync
    {
        internal const string MUTEX_HEAD = "Dpm.";

        internal static string GetMutexName(string name, bool isGlobal)
        {
            string mutexName = MUTEX_HEAD;

            if (isGlobal == true)
            {
                mutexName = string.Concat(@"Global\", mutexName);
            }

            mutexName = string.Concat(mutexName, name);

            return mutexName;
        }


        public static Mutex CreateMasterMutex(string name ,bool isGlobal = true)
        {
            Mutex mutex;
            string mutexName = GetMutexName(name, isGlobal);

            MutexSecurity mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule("everyone", MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            try
            {
                bool createNew;
                mutex = new Mutex(false, mutexName, out createNew, mutexSecurity);
                if (createNew != true)
                {
                    // 所有者になれなかった
                    mutex.Close();
                    mutex.Dispose();
                    throw new IOException($"A mutex named '{mutexName}' already existed.");
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
