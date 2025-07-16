using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery
{
    public interface IMyNaatiUserRefreshCacheQueryService : IUsersToRefreshQueryService<int, MyNaatiUserRefreshDto>
    {
    }

    public class MyNaatiUserRefreshDto : UserRefreshDto<int>
    {

    }
}
