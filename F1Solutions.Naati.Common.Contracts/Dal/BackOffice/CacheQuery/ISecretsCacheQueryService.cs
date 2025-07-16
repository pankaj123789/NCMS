using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Contracts.Dal.CacheQuery
{
    public interface ISecretsCacheQueryService : ICacheQueryService
    {
        string Get(string key);
    }
}
