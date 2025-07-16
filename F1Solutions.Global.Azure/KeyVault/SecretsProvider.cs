using F1Solutions.Naati.Common.Contracts.Security;
using Microsoft.Azure.KeyVault;
using System;
using System.Configuration;
using System.Threading.Tasks;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Naati.Common.Azure.KeyVault
{
    public class SecretsProvider : BaseKeyVault, ISecretsProvider
    {
        private readonly string _keyPrefix;
        public async Task<string> GetAsync(string key)
        {
            var prefixedKey = $"{_keyPrefix}{key}";
            try
            {
                var secret = await GetClient().GetSecretAsync(VaultUrl, prefixedKey).ConfigureAwait(false);
                return secret.Value;
            }
            catch (Exception ex)
            {
                var exception = new ApplicationException($"Could not get value for secret {prefixedKey}", ex);
                LoggingHelper.LogException(exception);
                throw exception;
            }
        }

        public string Get(string key)
        {
             var task = GetAsync(key);
             return task.Result;
        }

        public SecretsProvider()
        {
            _keyPrefix = ConfigurationManager.AppSettings["SecretPrefix"];
        }
    }
}
