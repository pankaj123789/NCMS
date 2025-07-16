using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Naati.Common.Bl.Security
{
    public abstract class PrivateApiAuthenticationAttribute : AuthorizeAttribute, IAuthenticationFilter
    {
        protected abstract string AuthenticationScheme { get; }
        protected abstract string DefaultUserName { get; }
        protected abstract string GetPrivateKey();
        private static bool? _debugMode;

        protected static bool DebugMode => (_debugMode ?? (_debugMode = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugMode"]))).Value;
        protected abstract IPrincipal GetPrincipal(string userName);

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (DebugMode)
            {
                LoggingHelper.LogWarning("Private authentication attribute started");
            }
           
            Stopwatch counter = null;
            string randomQueryId = null;
            try
            {
                if (DebugMode)
                {
                    counter = Stopwatch.StartNew();
                    randomQueryId = Guid.NewGuid().ToString();
                    LoggingHelper.LogWarning("{randomQueryId} OnAuthorization started: {Path},  min/ss/mmm {startDate}",randomQueryId,context.Request.RequestUri.AbsoluteUri,$"{DateTime.Now.Minute}/{DateTime.Now.Second}{DateTime.Now.Millisecond}");
                }

                return Authenticate(context, cancellationToken);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Private authentication error. url {url}", context?.Request?.RequestUri.AbsoluteUri);
                return Task.FromResult(0);
            }
            finally
            {
                if (counter != null)
                {
                    counter.Stop();
                    LoggingHelper.LogWarning(" {randomQueryId}  OnAuthorization finished: {Path},  min/ss/mmm {endDate}, duration ms: {duration}", randomQueryId, context?.Request?.RequestUri.AbsoluteUri, $"{DateTime.Now.Minute}/{DateTime.Now.Second}{DateTime.Now.Millisecond}", counter.Elapsed.TotalMilliseconds);
                }
            }
        }

        private Task Authenticate(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if ((context.Request.Headers.Authorization == null) || !AuthenticationScheme.Equals(context.Request.Headers.Authorization.Scheme,StringComparison.OrdinalIgnoreCase))
            {
                LoggingHelper.LogWarning("An invalid/inauthentic API call received", "(unknown)");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            var (appId, incomingBase64Signature, nonce, requestTimeStamp) =
                HmacCalculatorHelper.GetAuthorizationHeaderValues(context.Request.Headers.Authorization.Parameter);

            if (string.IsNullOrWhiteSpace(appId))
            {
                LoggingHelper.LogWarning(
                    "An invalid/inauthentic API call was received from {UserName}",
                    "(unknown)");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            var privateKey = GetPrivateKey();
            var unprotectedKey = HmacCalculatorHelper.UnProtectKey(privateKey);

            if (string.IsNullOrWhiteSpace(unprotectedKey))
            {
                LoggingHelper.LogWarning(
                    "An invalid/inauthentic API call was received from {InstitutionName}",
                    "[unknown]");
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            var validationResult = HmacCalculatorHelper.IsValidRequest(
                context.Request,
                appId,
                incomingBase64Signature,
                nonce,
                requestTimeStamp,
                unprotectedKey, false);

            if (!validationResult.Result)
            {
                LoggingHelper.LogWarning("An invalid/inauthentic API call was received from {UserName}", DefaultUserName);
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            var currentPrincipal = GetPrincipal(DefaultUserName);

            //Todo: Fix this
            context.Principal = currentPrincipal;
            Thread.CurrentPrincipal = currentPrincipal;
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result, AuthenticationScheme);
            return Task.FromResult(0);
        }

        public override bool AllowMultiple => false;

    }

    public enum EndpointCaller
    {
        Ncms = 1,
        MyNaati = 2
    }
}
