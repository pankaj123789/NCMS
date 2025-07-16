using System;
using System.Collections.Generic;
using F1Solutions.Global.Common.Logging;
using PayPal.Api;

namespace F1Solutions.Global.Common
{
    public static class PayPalConfigurationHelper
    {
        private static Dictionary<string, string> config;
        private static bool isInitialised { get; set; } = false;
        public static void ConfigurePayPal(string payPalMode, string payPalConnectionTimeout, string payPalRequestRetries, string payPalClientId, string payPalClientSecret)
        {
            config = new Dictionary<string, string>
            {
                { "mode", payPalMode },
                { "connectionTimeout", payPalConnectionTimeout },
                { "requestRetries", payPalRequestRetries },
                { "clientId", payPalClientId },
                { "clientSecret", payPalClientSecret }
            };
            isInitialised = true;
        }
        private static string GetAccessToken()
        {
            LoggingHelper.LogInfo($"PayPal GetAccessToken - Mode {config["mode"]}");

            string accessToken = new OAuthTokenCredential(config["clientId"], config["clientSecret"], config).GetAccessToken();
            return accessToken;
        }
        public static APIContext GetAPIContext()
        {
            if(!isInitialised)
            {
                throw new Exception("PayPalConfigurationHelper has not been initialised. Can't continue");
            }
            // return apicontext object by invoking it with the accesstoken  
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = config;
            return apiContext;
        }
    }
}
