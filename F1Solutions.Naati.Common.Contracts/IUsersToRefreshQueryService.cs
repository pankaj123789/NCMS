using System.Collections.Concurrent;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Contracts
{
    public interface IUsersToRefreshQueryService<in TIdentifierKey, TResultType> : ICacheQueryService where TResultType : UserRefreshDto<TIdentifierKey>
    {
        int RegisterUserCacheRefreshJob(TIdentifierKey userIdentifier, string invalidCookie);
        int RegisterUserHubNotificationJob(TIdentifierKey userIdentifier, int notificationId);

        void DeRegisterPendingUsers(IReadOnlyList<TResultType> user);
        IReadOnlyList<TResultType> GetUsersToRefresh();
        int GetDefaultRefreshDelay();
    }

    public enum UserRefreshType
    {
        Cache = 1,
        Notification = 2
    }
}
