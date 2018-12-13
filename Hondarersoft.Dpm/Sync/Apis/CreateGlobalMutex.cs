using System.Security.AccessControl;
using System.Threading;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Sync
    {
        public static Mutex CreateGlobalMutex(string name)
        {
            Mutex mutex;
            string mutexName = GetMutexName(name, true);

            MutexSecurity mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule("Everyone", MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));

            bool createNew;
            mutex = new Mutex(false, mutexName, out createNew, mutexSecurity);

            return mutex;
        }
    }
}
