using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Security;
using Hangfire.Dashboard;
using Ncms.Bl;
using Ncms.Bl.Security;
using Ncms.Contracts;

namespace Ncms.Ui.Security
{
    public class NcmsHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var cookie = HttpContext.Current.Request.Cookies["NCMS"]?.Value;

            if (cookie != null)
            {
                var formsTicket = FormsAuthentication.Decrypt(cookie);
                if (formsTicket != null && !formsTicket.Expired)
                {
                    HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(formsTicket), null);
                    Thread.CurrentPrincipal = HttpContext.Current.User;
                    var user = HttpContext.Current.User;
                    if (user.Identity.IsAuthenticated)
                    {
                        var principle = new NcmsPrincipal(user.Identity);
                        Thread.CurrentPrincipal = principle;
                        HttpContext.Current.User = principle;
                        var hasPermission = ServiceLocator.Resolve<IUserService>().HasPermission(
                            SecurityNounName.Dashboard,
                            SecurityVerbName.Manage);
                        return hasPermission;
                    }
                }
            }

            return false;
        }
    }
}