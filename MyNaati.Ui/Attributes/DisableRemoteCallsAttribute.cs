using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using F1Solutions.Global.Common.Logging;

namespace MyNaati.Ui.Attributes
{
    public class DisableRemoteCallsAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            LoggingHelper.LogInfo("MYNAATI DisableRemoteCallsAttribute Called");
            if (!HttpContext.Current.Request.IsLocal)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if ((filterContext.Result == null) || filterContext.Result is HttpUnauthorizedResult)
            {
                string actionName = filterContext.RouteData?.Values["action"]?.ToString();
                string controllerName = filterContext.RouteData?.Values["controller"]?.ToString();
                LoggingHelper.LogError($"Action '{actionName}' on controller {controllerName} tried to access by user host name {filterContext.HttpContext.Request.UserHostName} and address {filterContext.HttpContext.Request.UserHostAddress}.");

                filterContext.Result = new RedirectResult("Home/Index");
            }
        }
    }
}