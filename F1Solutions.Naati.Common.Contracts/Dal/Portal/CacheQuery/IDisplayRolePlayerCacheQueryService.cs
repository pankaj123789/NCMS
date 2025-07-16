using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery
{
    public interface IDisplayRolePlayerCacheQueryService : ICacheQueryService
    {
        bool IsRolePlayer(int naatiNumber);
        void RefreshDisplayRolePlayersFlag(int naatiNumber);
    }
}
