using System.Security.Principal;

namespace Hondarersoft.Dpm.Apis
{
    public static partial class Principal
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
