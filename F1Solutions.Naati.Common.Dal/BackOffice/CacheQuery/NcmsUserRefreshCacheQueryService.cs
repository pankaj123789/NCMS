using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.CacheQuery;

namespace F1Solutions.Naati.Common.Dal.BackOffice.CacheQuery
{
    public class NcmsUserRefreshCacheQueryService : BaseUsersToRefreshCacheQueryService<string, NcmsUserRefreshDto>, INcmsUserRefreshCacheQueryService
    {
        protected override NcmsUserRefreshDto CreateInstance()
        {
           return new NcmsUserRefreshDto();
        }

        protected override string TransformKey(string key)
        {
            return key.ToUpper();
        }
    }
}
