using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Dal
{
    public abstract class TokenAuthorisationService : ITokenAuthorisationService
    {
        protected abstract string AccessTokenSystemKey { get; }
        protected abstract string ClientIdSecretKey { get; }
        protected abstract string ResourceKey { get; }
        protected abstract string TokenUrlKey { get; }
        protected abstract string RedirectUriKey { get; }
        protected abstract Task<HttpResponseMessage> RefreshToken(string refreshToken);

        protected string ClientId { get; }
        protected string ResourceId { get; }
        protected string GraphTokenUrl { get; }

        protected ISystemQueryService SystemQueryService { get; }
        protected ISecretsCacheQueryService SecretsProvider { get; }

        protected abstract string ClientSecret { get; }

        protected HttpClient CreateClient()
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 30);
            return httpClient;
        }

        public TokenAuthorisationService(ISystemQueryService systemQueryService, ISecretsCacheQueryService secretsProvider, bool checkToken = true)
        {
            SystemQueryService = systemQueryService;
            SecretsProvider = secretsProvider;
            ClientId = secretsProvider.Get(ClientIdSecretKey);
            ResourceId = ConfigurationManager.AppSettings[ResourceKey];
            GraphTokenUrl = ConfigurationManager.AppSettings[TokenUrlKey];

            // todo use a certificate instead of a secret
            if (checkToken)
            {
                CheckRefreshAccessToken();
            }
        }

        public void CheckRefreshAccessToken()
        {
            var tokenStatus = GetTokenStatus();
            if (tokenStatus == AccessTokenStatus.None)
            {
                tokenStatus = OnAuthenticationRequired();
            }
            if (tokenStatus == AccessTokenStatus.Expired)
            {
                tokenStatus = OnTokenExpired();
            }
            if (tokenStatus != AccessTokenStatus.Valid)
            {
                throw new WebServiceException($"Unable to obtain new access token for {this.GetType()}");
            }
        }

        protected virtual AccessTokenStatus OnAuthenticationRequired()
        {
            throw new WebServiceException("Authentication required.");
        }

        protected virtual AccessTokenStatus OnTokenExpired()
        {
            var token = RefreshToken();
            SystemQueryService.SetSystemValue(new SetSystemValueRequest() { ValueKey = AccessTokenSystemKey, Value = token });
            return GetTokenStatus();
        }

        protected string ValidateAccessAndGetToken(string accessCode)
        {
            try
            {
                accessCode.NotNullOrWhiteSpace(nameof(accessCode));
                var issued = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var values = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "client_id", ClientId },
                    { "code", accessCode },
                    { "redirect_uri", ConfigurationManager.AppSettings[RedirectUriKey] },
                    { "resource", ResourceId },
                    { "client_secret", ClientSecret }
                };
                var content = new FormUrlEncodedContent(values);

                LoggingHelper.LogInfo($"Before Auth ValidateAccess {GraphTokenUrl}");
                var response = CreateClient().PostAsync(GraphTokenUrl, content).Result;
                LoggingHelper.LogInfo($"Afer Auth ValidateAccess");


                var responseString = response.Content.ReadAsStringAsync().Result;
                var responseContent = JsonConvert.DeserializeObject<dynamic>(responseString);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    LoggingHelper.LogError("Failed to obtain Graph access token: {Error} - {ErrorDescription}", responseContent.error, responseContent.error_description);
                    throw new Exception($"{responseContent.error} - {responseContent.error_description}");
                }

                responseContent.issued = issued;
                return JsonConvert.SerializeObject(responseContent);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error obtaining Graph token");
                throw;
            }
        }

        public string GetToken(string accessCode)
        {
            try
            {
                var responseString = ValidateAccessAndGetToken(accessCode);

                var plainTextBytes = Encoding.Unicode.GetBytes(responseString);
                var encrypted = MachineKey.Protect(plainTextBytes, "oauth2");

                return Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error obtaining Graph token");
                throw;
            }
        }

        protected Dictionary<string, string> GetTokenStoredResponse()
        {
            var sysVal =
                SystemQueryService.GetSystemValue(new GetSystemValueRequest
                {
                    ValueKey = AccessTokenSystemKey
                });

            if (!string.IsNullOrEmpty(sysVal.Value))
            {
                var plainTextBytes = Convert.FromBase64String(sysVal.Value);
                var decrypted = MachineKey.Unprotect(plainTextBytes, "oauth2");
                if (decrypted != null)
                {
                    var value = Encoding.UTF8.GetString(decrypted).Replace("\0", "");
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                }
            }
            return null;
        }

        protected virtual string GetTokenType()
        {
            var tokenResponse = GetTokenStoredResponse();
            return tokenResponse == null ? null : tokenResponse["token_type"];
        }

        public virtual string GetAccessToken()
        {
            var tokenResponse = GetTokenStoredResponse();
            return tokenResponse == null ? null : tokenResponse["access_token"];
        }

        public virtual string GetFreshAccessToken()
        {
           CheckRefreshAccessToken();
           return GetAccessToken();
        }

        protected virtual string GetRefreshToken()
        {
            var tokenResponse = GetTokenStoredResponse();
            if (tokenResponse == null)
            {
                throw new Exception("Unable to refresh token: no token previously saved.");
            }

            var refreshToken = tokenResponse["refresh_token"];
            return refreshToken;
        }

        protected virtual string RefreshToken()
        {
            try
            {
                var refreshToken = GetRefreshToken();
                var issued = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var response = RefreshToken(refreshToken).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                var responseContent = JsonConvert.DeserializeObject<dynamic>(responseString);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    LoggingHelper.LogError("Failed to refresh Graph access token: {Error}", responseString);
                    throw new Exception(responseString);
                }

                responseContent.issued = issued;
                var plainTextBytes = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(responseContent));
                var encrypted = MachineKey.Protect(plainTextBytes, "oauth2");

                return Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error refreshing Graph token");
                throw;
            }
        }

        public AccessTokenStatus GetTokenStatus()
        {
            var tokenResponse = GetTokenStoredResponse();
            if (tokenResponse == null)
            {
                return AccessTokenStatus.None;
            }
            string expiryString;
            if (tokenResponse.ContainsKey("expires_on"))
            {
                expiryString = tokenResponse["expires_on"];
            }
            else
            {
                var issued = Convert.ToInt64(tokenResponse["issued"]);
                var expiresIn = Convert.ToInt64(tokenResponse["expires_in"]);
                expiryString = $"{issued + expiresIn}";
            }
            var expiry = new DateTime(1970, 1, 1).AddSeconds(Int64.Parse(expiryString)).ToLocalTime();
            var timeRemaining = expiry - DateTime.Now;
            return timeRemaining.TotalMinutes > 1
                ? AccessTokenStatus.Valid
                : AccessTokenStatus.Expired;
        }

        public string GetBaseAddress()
        {
            return ResourceId;
        }
    }

    public enum AccessTokenStatus
    {
        None = 0,
        Expired = 1,
        Valid = 2
    }
}
