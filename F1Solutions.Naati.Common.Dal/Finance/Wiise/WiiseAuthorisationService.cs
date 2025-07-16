using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public class WiiseAuthorisationService: TokenAuthorisationService,IWiiseAuthorisationService
    {
        public WiiseAuthorisationService(ISystemQueryService systemQueryService, ISecretsCacheQueryService secretsProvider, bool checkToken = false) :
            base(systemQueryService, secretsProvider, checkToken)
        {
        }

        private string _clientSecret;
        protected override string ClientSecret => _clientSecret ?? (_clientSecret = GetClientSecret());

        protected override string AccessTokenSystemKey => "WiiseAccessToken";
        protected override string ClientIdSecretKey => "WiiseAuthClientId";

        protected override string ResourceKey => "WiiseResource";

        protected override string TokenUrlKey => "WiiseTokenUrl";

        protected override string RedirectUriKey => "WiiseAuthRedirectUri";

        protected override Task<HttpResponseMessage> RefreshToken(string refreshToken)
        {
            var values = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken },
                };
            var content = new FormUrlEncodedContent(values);
            var plainTextBytes = Encoding.Default.GetBytes($"{ClientId}:{ClientSecret}");
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(plainTextBytes));
            return client.PostAsync(GraphTokenUrl, content);
        }

        public string GetTenant()
        {
            return SystemQueryService.GetSystemValue(new GetSystemValueRequest
            {
                ValueKey = "WiiseTenantId"
            }).Value;
        }


        public string GetTokenAndTenant(string accessCode, out string tenantId)
        {
            var responseString = ValidateAccessAndGetToken(accessCode);
            var authResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
            tenantId = SecretsProvider.Get("WiiseAuthTenant");
            return Encrypt(responseString);
        }

        private string GetClientSecret() => SecretsProvider.Get("WiiseClientSecret");

        private static string Encrypt(string token)
        {
            var plainTextBytes = Encoding.Unicode.GetBytes(token);
            var encrypted = MachineKey.Protect(plainTextBytes, "oauth2");
            return Convert.ToBase64String(encrypted);
        }

    }

}
