using F1Solutions.Global.Common.Logging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiThrottle;

namespace MyNaati.Ui.Helpers
{
    public class MyNaatiThrottlingHandler:ThrottlingHandler
    {
        public MyNaatiThrottlingHandler(ThrottlePolicy policy, 
            PolicyCacheRepository policyRepository, 
            CacheRepository repository, 
            MyNaatiThrottlingCustomAddressParser ipAddressParser,
            TracingThrottleLogger logger) :base(policy,policyRepository, repository,logger, ipAddressParser)
        {

        }

        protected override Task<HttpResponseMessage> QuotaExceededResponse(HttpRequestMessage request, object content, HttpStatusCode responseCode, string retryAfter)
        {
            var remoteIpAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(remoteIpAddress))
            {
                remoteIpAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            LoggingHelper.LogInfo($"API throttling activated for {request.RequestUri} for IP address {remoteIpAddress}.");
            return base.QuotaExceededResponse(request, content, responseCode, retryAfter);
        }
    }
}