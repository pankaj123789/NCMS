using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Security;

namespace F1Solutions.Naati.Common.Dal.CacheQuery
{
    public class SecretsCacheQueryService : BaseLazyCacheQueryService<string, string, string>, ISecretsCacheQueryService
    {
        private readonly ISecretsProvider _secretsProvider;

        public SecretsCacheQueryService(ISecretsProvider secretsProvider)
        {
            _secretsProvider = secretsProvider;
        }

        public string Get(string key)
        {
            return GetItem(key);
        }

        protected override string GetDbSingleValue(string key)
        {
            return _secretsProvider.Get(key);
        }

        protected override string MapToTResultType(string item) => item;

        protected override string TransformKey(string key) => key;
    }
}
