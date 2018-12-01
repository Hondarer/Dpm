using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm
{
    public static partial class Apis
    {
        private static object lock_IsAdministrator = new object();

        private static bool? isAdministrator = null;

        public static bool IsAdministrator()
        {
            if (isAdministrator == null)
            {
                lock (lock_IsAdministrator)
                {
                    if (isAdministrator == null)
                    {
                        WindowsIdentity identity = WindowsIdentity.GetCurrent();
                        WindowsPrincipal principal = new WindowsPrincipal(identity);
                        isAdministrator = principal.IsInRole(WindowsBuiltInRole.Administrator);
                    }
                }
            }

            return (bool)isAdministrator;
        }
    }
}
