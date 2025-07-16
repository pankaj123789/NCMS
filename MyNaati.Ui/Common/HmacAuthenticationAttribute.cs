using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.BackOffice;
using MyNaati.Ui.Security;

namespace MyNaati.Ui.Common
{
    public class HmacAuthenticationAttribute : Attribute, IAuthenticationFilter, IAuthorizationFilter
    {
        private const string AuthenticationScheme = "Naati";
        private readonly MyNaati.Contracts.BackOffice.EndpointPermission _requiredPermission;

        public HmacAuthenticationAttribute(MyNaati.Contracts.BackOffice.EndpointPermission permission)
        {
            _requiredPermission = permission;
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
         
            if ((context.Request.Headers.Authorization == null) || !AuthenticationScheme.Equals(context.Request.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                LoggingHelper.LogWarning($"schema: {context.Request.Headers.Authorization}, expected : {AuthenticationScheme}");
                LoggingHelper.LogWarning("An invalid/inauthentic API call received (s)", "(unknown)");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            var (appId, incomingBase64Signature, nonce, requestTimeStamp) = HmacCalculatorHelper.GetAuthorizationHeaderValues(context.Request.Headers.Authorization.Parameter);

            if (string.IsNullOrWhiteSpace(appId))
            {
                LoggingHelper.LogWarning("An invalid/inauthentic API call was received from {InstitutionName}", "(unknown) (auth)");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            var service = ServiceLocator.Resolve<IApiAccessService>();
            var apiAccess = service.GetApiAccess(appId);

            if (apiAccess == null)
            {
                LoggingHelper.LogWarning("An invalid/inauthentic API call was received from {InstitutionName}", "(unknown) (db auth");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            if (apiAccess.Inactive)
            {
                LoggingHelper.LogWarning("An API call from {InstitutionName} was refused because their access key is currently Inactive", apiAccess.InstitutionName);
                context.ErrorResult = new StatusCodeResult(HttpStatusCode.Forbidden, context.Request);
                return Task.FromResult(0);
            }

            var validationResult = HmacCalculatorHelper.IsValidRequest(context.Request, appId, incomingBase64Signature, nonce,requestTimeStamp, apiAccess.PrivateKey, true);

            if (!validationResult.Result)
            {
                LoggingHelper.LogWarning("An invalid/inauthentic API call was received from {InstitutionName}", apiAccess.InstitutionName );
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            if ((apiAccess.Permissions & (int)_requiredPermission) == 0)
            {
                LoggingHelper.LogWarning(
                    "An API call from {InstitutionName} was refused because they do not have the required permissions", apiAccess.InstitutionName);
                context.ErrorResult = new StatusCodeResult(HttpStatusCode.Forbidden, context.Request);
                return Task.FromResult(0);
            }

            var currentPrincipal = new GenericPrincipal(new GenericIdentity(apiAccess.InstitutionName), null);
            context.Principal = currentPrincipal;
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result, AuthenticationScheme);
            return Task.FromResult(0);
        }

        public bool AllowMultiple => false;
        
        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken,Func<Task<HttpResponseMessage>> continuation)
        {
            return continuation();
        }
    }



   
}