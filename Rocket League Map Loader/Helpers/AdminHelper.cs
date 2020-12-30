using System.Security.Principal;

namespace RL_Map_Loader.Helpers
{
    public class AdminHelper
    {
        public static bool IsAdmin()
        {
            try
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                    .IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }
}
