using System.Net;
using System.Net.Http;
using WebApiThrottle.Net;
using F1Solutions.Global.Common.Logging;

namespace MyNaati.Ui.Helpers
{
    public class MyNaatiThrottlingCustomAddressParser : DefaultIpAddressParser
    {
        public override IPAddress GetClientIp(HttpRequestMessage request)
        {
            var remoteIpAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if a value has been sent in HTTP_X_FORWARDED_FOR then Azure will add to it, so we take away what they sent in
            if (remoteIpAddress != null && remoteIpAddress.Contains(","))
            {
                var bothIpAddresses = remoteIpAddress.Split(',');
                var lastAddressIndex = bothIpAddresses.Length - 1;
                remoteIpAddress = bothIpAddresses[lastAddressIndex];
            }

            //LoggingHelper.LogInfo($"HTTP_X_FORWARDED_FOR is '{remoteIpAddress}'");

            if (string.IsNullOrEmpty(remoteIpAddress))
            {
                remoteIpAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ParseIp(remoteIpAddress);
        }
    }
}