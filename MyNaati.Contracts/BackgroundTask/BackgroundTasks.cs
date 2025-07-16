using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;

namespace MyNaati.Contracts.BackgroundTask
{
    public interface IMyNaatiRefreshPendingUsersTask : IBackgroundTask
    {
        int RegisterUserCacheRefresh(int naatiNumber, string invalidCookie, DateTime cookieExpiryDate);
        void RefreshLocalUserCache(IEnumerable<MyNaatiUserRefreshDto> users);
    }

    public interface IMyNaatiRefreshAllUsersTask : IBackgroundTask
    {
        void RefreshAllUsersLocalCache();
    }

    public interface IMyNaatiRefreshCookieTask : IBackgroundTask
    {
        void RefreshAllInvalidLocalCookies();
    }
    public interface IMyNaatiRefreshSystemCacheTask : IBackgroundTask
    {
        void RefreshLocalSystemCache();
    }

    public enum MyNaatiJobTypeName
    {
        MyNaatiRefreshSystemCache,
        MyNaatiRefreshAllUsersCache,
        MyNaatiRefreshCookieCache,
        MyNaatiRefreshPendingUsersCache,
    }

    public static class BackgroundTasksParameters
    {
        public const string ServerName = "ServerName";
    }
}
