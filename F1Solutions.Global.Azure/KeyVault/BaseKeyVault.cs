using F1Solutions.Naati.Common.Contracts.Security;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Naati.Common.Azure.KeyVault
{
    public class BaseKeyVault
    {
        private string ClientId => Environment.GetEnvironmentVariable("KeyVaultClientId");
        private string ClientSecret => Environment.GetEnvironmentVariable("KeyVaultClientSecret");
        protected string VaultUrl => Environment.GetEnvironmentVariable("KeyVaultUrl");

        public KeyVaultClient GetClient()
        {
            return new KeyVaultClient(GetToken);
        }

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(ClientId, ClientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred).ConfigureAwait(false);

            if (result == null)
            {
                var exception = new InvalidOperationException("Failed to obtain the JWT token");
                LoggingHelper.LogException(exception);
                throw exception;
            }
            return result.AccessToken;
        }
    }
}
