using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Bl.Security;
using Ncms.Contracts;
using Ninject;

namespace Ncms.Ui.Security
{
    public class NcmsAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly List<(SecurityVerbName, SecurityNounName)> _requiredPermissions;

        private static bool? _debugMode;

        protected static bool DebugMode => (_debugMode ?? (_debugMode = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugMode"]))).Value;

        public NcmsAuthorizeAttribute(SecurityVerbName verb, params SecurityNounName[] nouns)
        {
            if (nouns == null || nouns.Length == 0)
            {
                throw new ArgumentException("At least one noun is required");
            }
            _requiredPermissions = nouns.Select(noun => ( verb, noun )).ToList();
        }

        public NcmsAuthorizeAttribute(SecurityVerbName[] verbs, SecurityNounName noun)
        {
            if (verbs == null || verbs.Length == 0)
            {
                throw new ArgumentException("At least one verb is required");
            }
            _requiredPermissions = verbs.Select(verb => (verb, noun)).ToList();
        }

        public NcmsAuthorizeAttribute(SecurityVerbName[] verbs, SecurityNounName[] nouns)
        {
            if (verbs == null || verbs.Length == 0 || nouns == null || nouns.Length == 0)
            {
                throw new ArgumentException("At least one verb and one noun is required");
            }

            if (verbs.Length != nouns.Length)
            {
                throw new ArgumentException("Number of verb must match number of nouns (or use another constructor)");
            }

            _requiredPermissions = verbs.Select((verb, i) => (verb, nouns[i])).ToList();
        }

        public NcmsAuthorizeAttribute(SecurityVerbName verb, SecurityNounName noun)
        {
            _requiredPermissions = new List<(SecurityVerbName, SecurityNounName)> { (verb, noun) };
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (DebugMode)
            {
                LoggingHelper.LogWarning("NcmsAuthorizeAttribute authentication attribute started");
            }
           
            Stopwatch counter = null;
            string randomQueryId = null;
            try
            {
                if (DebugMode)
                {
                    counter = Stopwatch.StartNew();
                    randomQueryId = Guid.NewGuid().ToString();
                    LoggingHelper.LogWarning("{randomQueryId} NcmsAuthorizeAttribute started: {Path},  min/ss/mmm {startDate}", randomQueryId, actionContext.Request.RequestUri.AbsoluteUri, $"{DateTime.Now.Minute}/{DateTime.Now.Second}{DateTime.Now.Millisecond}");
                }

                CheckAuthorization(actionContext);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "NcmsAuthorizeAttribute error. url {url}", actionContext?.Request?.RequestUri.AbsoluteUri);
            }
            finally
            {
                if (counter != null)
                {
                    counter.Stop();
                    LoggingHelper.LogWarning(" {randomQueryId}  NcmsAuthorizeAttribute finished: {Path},  min/ss/mmm {endDate}, duration ms: {duration}", randomQueryId, actionContext?.Request?.RequestUri.AbsoluteUri, $"{DateTime.Now.Minute}/{DateTime.Now.Second}{DateTime.Now.Millisecond}", counter.Elapsed.TotalMilliseconds);
                }
            }
        }

        public void CheckAuthorization(HttpActionContext actionContext)
        {
            if (Thread.CurrentPrincipal.Identity == null)
            {
                throw new Exception($"No authenticated {Naati.Resources.Shared.AppName} user");
            }

            var permissions = (Thread.CurrentPrincipal as NcmsPrincipal)?.Permissions;

            // if the user has ANY of the required rights, authorise the request
            bool isAuthorised = permissions != null &&
                _requiredPermissions.Any(right => permissions.Any(x => right.Item2 == x.Noun &&
                                                             ((int)right.Item1 & x.Permissions) == (int)right.Item1));

            // FOR DEV ONLY
            //LoggingHelper.LogDebug("Access check: {@RequiredPermissions}: {Path}", _requiredPermissions, actionContext.Request.RequestUri.AbsoluteUri);

            if (isAuthorised)
            {
                if (DebugMode)
                {
                    LoggingHelper.LogInfo("Access granted: {Path}, {@RequiredPermissions}", actionContext.Request.RequestUri.AbsoluteUri, _requiredPermissions);
                }

                base.OnAuthorization(actionContext);
            }
            else
            {
                LoggingHelper.LogWarning("Access denied: {Path}, {@RequiredPermissions}", actionContext.Request.RequestUri.AbsoluteUri, _requiredPermissions);
                HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            actionContext.Response.ReasonPhrase = Naati.Resources.Server.UnauthorisedAccess;
        }
    }
}