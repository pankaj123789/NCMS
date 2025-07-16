using System.Web.Mvc;
using System.Web.WebPages;

namespace MyNaati.Ui.Helpers
{
    public static class MvcExtensions
    {
        private const string RouteDataControllerKey = "controller";
        private const string RouteDataActionKey = "action";

        public static string CurrentController(this ViewContext context)
        {
            return (string)context.RouteData.Values[RouteDataControllerKey];
        }

        public static string CurrentAction(this ViewContext context)
        {
            return (string)context.RouteData.Values[RouteDataActionKey];
        }

        public static bool IsInController(this ViewContext context, string controller)
        {
            return string.Equals(context.CurrentController(), controller);
        }

        public static bool IsInAction(this ViewContext context, string action)
        {
            return string.Equals(context.CurrentAction(), action);
        }

        public static MvcHtmlString ToMvcHtmlString(this string value)
        {
            return MvcHtmlString.Create(value);
        }

        public static MvcHtmlString ToMvcHtmlString(this HelperResult result)
        {
            return MvcHtmlString.Create(result.ToString());
        }
    }
}
