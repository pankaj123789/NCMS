using System.Linq;
using System.Reflection;

namespace Ncms.Ui.Helpers
{
    public static class ViewHelper
    {
        public static string GetVersionInfo()
        {
            return Assembly.GetAssembly(typeof(ViewHelper))
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
                .Cast<AssemblyFileVersionAttribute>()
                .FirstOrDefault()?.Version ?? string.Empty;
        }
    }
}
