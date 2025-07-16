using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery
{
    public interface INcmsUserRefreshCacheQueryService : IUsersToRefreshQueryService<string, NcmsUserRefreshDto>
    {
    }

    public class NcmsUserRefreshDto : UserRefreshDto<string> { }
   
}
