using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;
using F1Solutions.Global.Common.Logging;

namespace Ncms.Ui.Security
{
    /// <summary>
    /// This attribute doesn't do Authorization checks. It just checks if the current user is authenticated.
    /// </summary>
    public class NcmsAuthenticationCheckAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Thread.CurrentPrincipal.Identity == null || !IsAuthenticated(actionContext))
            {
                HandleUnauthenticatedUser(actionContext);
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }

        private void HandleUnauthenticatedUser(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            actionContext.Response.ReasonPhrase = $"No authenticated {Naati.Resources.Shared.AppName} user";
        }

        private bool IsAuthenticated(HttpActionContext actionContext)
        {
            try
            {
                Collection<CookieHeaderValue> cookieHeaders = actionContext.Request.Headers.GetCookies();
                if (cookieHeaders.Count == 1)
                {
                    var authCookie = cookieHeaders[0].Cookies.SingleOrDefault(x => x.Name == "NCMS");
                    if (authCookie != null)
                    {
                        var formsTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        return formsTicket != null && !formsTicket.Expired;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error checking NCMS authentication ticket: {Message}", ex.Message);
            }

            return false;
        }
    }
}