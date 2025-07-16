using System.Configuration;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Security;

namespace F1Solutions.Naati.Common.Bl.Security
{
    public class SecretsProvider : ISecretsProvider
    {
        public Task<string> GetAsync(string key)
        {
            return Task.Run(() => Get(key));
        }
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}