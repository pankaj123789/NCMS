using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Dal.CacheQuery;

namespace F1Solutions.Naati.Common.Dal.Portal.CacheQuery
{
    public class MyNaatiUserRefreshCacheQueryService : BaseUsersToRefreshCacheQueryService<int, MyNaatiUserRefreshDto>, IMyNaatiUserRefreshCacheQueryService
    {
        protected override MyNaatiUserRefreshDto CreateInstance()
        {
            return new MyNaatiUserRefreshDto();
        }
    }

   
}
