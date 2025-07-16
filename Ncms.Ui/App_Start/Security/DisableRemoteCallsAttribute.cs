using F1Solutions.Global.Common.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl;
using Ncms.Bl.Security;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;

namespace Ncms.Ui.Security
{
    public class DisableRemoteCallsAttribute : FilterAttribute, IActionFilter
    {
        public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            LoggingHelper.LogInfo("NCMS DisableRemoteCallsAttribute Called");
            if (!HttpContext.Current.Request.IsLocal)
            {
                string actionName = actionContext.ControllerContext.RouteData?.Values["action"]?.ToString();
                string controllerName = actionContext.ControllerContext.RouteData?.Values["controller"]?.ToString();
                LoggingHelper.LogError($"Action '{actionName}' on controller {controllerName} tried to access by {actionContext.Request.RequestUri.Host}.");
                return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
            }
            var service = ServiceLocator.Resolve<ISecretsCacheQueryService>();
            var defaultIdentity = service.Get(SecuritySettings.NcmsDefaultIdentityKey);
            new NcmsPrincipal(defaultIdentity);
            return continuation();
        }
    }
}