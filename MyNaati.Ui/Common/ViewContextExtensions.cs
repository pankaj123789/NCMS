using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MyNaati.Ui.Common
{
	public static class ViewContextExtensions
	{
		/// <summary>
		/// Gets a value specifying if Google Analytics is enabled. Controlled by web.config
		/// </summary>
		public static bool IsGoogleAnalyticsEnabled(this ViewContext context)
		{
			return WebConfigurationManager.AppSettings["GoogleAnalyticsEnabled"] == "true";
		}

		public static string GetVersionInfo(this ViewContext context)
		{
			return Assembly.GetAssembly(typeof(ViewContextExtensions))
				.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
				.Cast<AssemblyFileVersionAttribute>()
				.FirstOrDefault()?.Version ?? string.Empty;
		}
	}
}