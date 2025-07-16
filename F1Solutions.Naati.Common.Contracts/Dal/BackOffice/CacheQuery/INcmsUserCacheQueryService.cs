using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery
{
    
    public interface INcmsUserCacheQueryService : ICacheQueryService
    {
        UserDetailsDto GetUser(string userName);
        void RefreshUserCache(string userName);
    }
}
