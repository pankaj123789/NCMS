using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.Payment;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public class SecurePayAuthorisationService : TokenAuthorisationService, ISecurePayAuthorisationService
    {
        public SecurePayAuthorisationService(ISystemQueryService systemQueryService, ISecretsCacheQueryService secretsProvider) : base(systemQueryService, secretsProvider, false)
        {
        }

        private string _clientSecret;
        private string _scope;
        protected override string AccessTokenSystemKey => "SecurePayAccessToken";

        protected string Scope => _scope ?? (_scope = ConfigurationManager.AppSettings["SecurePayScope"]);

        protected override string ClientIdSecretKey => "SecurePayClientId";

        protected override string ResourceKey => "SecurePayResource";

        protected override string TokenUrlKey => "SecurePayTokenUrl";

        protected override string RedirectUriKey => "SecurePayRedirectUri";

        protected override string ClientSecret => _clientSecret ?? (_clientSecret = GetClientSecret());

        protected override Task<HttpResponseMessage> RefreshToken(string refreshToken)
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                //{ "scope", Scope },
                { "audience", "https://api.payments.auspost.com.au"}
            };
            var content = new FormUrlEncodedContent(values);
            var plainTextBytes = Encoding.Default.GetBytes($"{ClientId}:{ClientSecret}");
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(plainTextBytes));
            
            LoggingHelper.LogInfo("Calling SecurePay refresh token..");
            return client.PostAsync(GraphTokenUrl, content);
        }

        private string GetClientSecret() => SecretsProvider.Get("SecurePayClientSecret");

        protected override AccessTokenStatus OnAuthenticationRequired()
        {
            return OnTokenExpired();
        }

        protected override string GetRefreshToken()
        {
            return null;
        }
    }
}
