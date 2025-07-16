using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace F1Solutions.Naati.Common.Dal.Office365
{
    public class AzureAuthorisationService : TokenAuthorisationService, IAzureAuthorisationService
    {
        public AzureAuthorisationService(ISystemQueryService systemQueryService, ISecretsCacheQueryService secretsProvider, bool checkToken = true) :
            base(systemQueryService, secretsProvider, checkToken)
        {
        }

        protected override Task<HttpResponseMessage> RefreshToken(string refreshToken)
        {
            var values = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", ClientId },
                    { "refresh_token", refreshToken },
                    { "resource", ResourceId },
                    { "client_secret", ClientSecret }
                };
            var content = new FormUrlEncodedContent(values);

            return CreateClient().PostAsync(GraphTokenUrl, content);
        }

        protected override string AccessTokenSystemKey => "MicrosoftGraphAccessToken";

        protected override string ClientIdSecretKey => "Office365AppId";

        protected override string ResourceKey => "GraphResourceId";

        protected override string TokenUrlKey => "GraphTokenUrl";

        protected override string RedirectUriKey => "AadAuthRedirectUri";

        private string _clientSecret;
        protected override string ClientSecret => _clientSecret ?? (_clientSecret = GetClientSecret());

        private string GetClientSecret()
        {
            var sysVal = SystemQueryService.GetSystemValue(new GetSystemValueRequest
            {
                ValueKey = "MicrosoftGraphAppKey"
            });

            if (!string.IsNullOrEmpty(sysVal.Value))
            {
                var plainTextBytes = Convert.FromBase64String(sysVal.Value);
                var decrypted = MachineKey.Unprotect(plainTextBytes);
                if (decrypted != null)
                {
                    return Encoding.UTF8.GetString(decrypted).Replace("\0", "");
                }
            }
            else
            {
                throw new WebServiceException("A graph app key could not be found.");
            }

            return "";
        }
    }
}
