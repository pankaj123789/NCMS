using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace MyNaati.Ui.Common
{
    public static class UrlExtensionMethods
    {
        public static string ExternalUrlFromConfig(this UrlHelper helper, string key)
        {
            string modifiedKey = "ExternalUrl_" + key;
            string url = ConfigurationManager.AppSettings[modifiedKey];

            if (string.IsNullOrEmpty(url))
            {
                return VirtualPathUtility.ToAbsolute("~/Home/ComingSoon");
            }

            if (VirtualPathUtility.IsAppRelative(url))
            {
                url = VirtualPathUtility.ToAbsolute(url);
            }

            return url;
        }
    }
}