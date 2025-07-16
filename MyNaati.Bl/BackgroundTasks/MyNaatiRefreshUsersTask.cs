using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using MyNaati.Contracts;
using MyNaati.Contracts.BackgroundTask;

namespace MyNaati.Bl.BackgroundTasks
{
    public class MyNaatiRefreshPendingUsersTask : BaseBackgroundTask, IMyNaatiRefreshPendingUsersTask
    {
        private readonly IMyNaatiPodsIntegrationService _podsIntegrationService;
        private readonly IUtilityQueryService _utilityQueryService;
        private readonly ICookieQueryService _cookieQueryService;
        private readonly IDisplayBillsCacheQueryService _displayBillsCacheQueryService;
        private readonly IDisplayRolePlayerCacheQueryService _displayRolePlayerCacheQueryService;
        private readonly IMyNaatiUserRefreshCacheQueryService _usersToRefreshService;

        public MyNaatiRefreshPendingUsersTask(ISystemQueryService systemQueryService,
            IMyNaatiPodsIntegrationService podsIntegrationService,
            IUtilityQueryService utilityQueryService, 
            IBackgroundTaskLogger backgroundTaskLogger, 
            ICookieQueryService cookieQueryService,
            IDisplayBillsCacheQueryService displayBillsCacheQueryService,
            IDisplayRolePlayerCacheQueryService displayRolePlayerCacheQueryService,
            IMyNaatiUserRefreshCacheQueryService usersToRefreshService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _podsIntegrationService = podsIntegrationService;
            _utilityQueryService = utilityQueryService;
            _cookieQueryService = cookieQueryService;
            _displayBillsCacheQueryService = displayBillsCacheQueryService;
            _displayRolePlayerCacheQueryService = displayRolePlayerCacheQueryService;
            _usersToRefreshService = usersToRefreshService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue(BackgroundTasksParameters.ServerName, value: out var serverName))
            {
                serverName = _utilityQueryService.GetSystemIp();
            }

            RefreshAllServerPendingUsers(serverName);
        }

        public int RegisterUserCacheRefresh(int naatiNumber, string invalidCookie, DateTime cookieExpiryDate)
        {
            TaskLogger.WriteInfo($"Refreshing MyNaati database server cookie for user {naatiNumber}...");
            // Add Cookie to db, so new server will be able to get existing invalid cookies
            _cookieQueryService.InvalidateDbCookie(invalidCookie, cookieExpiryDate);

            var jobDelay = _usersToRefreshService.RegisterUserCacheRefreshJob(naatiNumber, invalidCookie);

            return jobDelay;
        }

        public void RefreshLocalUserCache(IEnumerable<MyNaatiUserRefreshDto> users)
        {
            var ip = _utilityQueryService.GetSystemIp();
            foreach (var user in users)
            {
                TaskLogger.WriteInfo("Refreshing user cache for user {cacheUserName} on server {serverIp}", user.Identifier, ip);
                _displayBillsCacheQueryService.RefreshDisplayBillsFlag(user.Identifier);
                _displayRolePlayerCacheQueryService.RefreshDisplayRolePlayersFlag(user.Identifier);
                foreach (var invalidCookie in user.InvalidCookies)
                {
                    _cookieQueryService.InvalidateCacheCookie(invalidCookie);
                }
            }
        }

        private void RefreshAllServerPendingUsers(string sourceServerName)
        {
            var localIp = _utilityQueryService.GetSystemIp();

            if (sourceServerName != localIp)
            {
                if (GetRemoteHangFireServers().All(x => x != sourceServerName))
                {
                    TaskLogger.WriteWarning($"Job {nameof(RefreshAllServerPendingUsers)} will not be executed because source server {sourceServerName} was not found");
                    return;
                }

                var callName = nameof(_podsIntegrationService.ExecuteRefreshPendingUserTaskAsync);
                TaskLogger.WriteInfo($"Redirecting job {callName} to server {sourceServerName}");
                var task = _podsIntegrationService.ExecuteRefreshPendingUserTaskAsync(sourceServerName);
                var completed = task.Wait(60000);

                TaskLogger.WriteInfo($"Redirect of job {callName} to server {sourceServerName} finished with completed status: {completed}.  Errors: {!task.IsCompleted || task.Exception != null} ");
                return;
            }

            var pendingUsers = _usersToRefreshService.GetUsersToRefresh();
            RefreshLocalUserCache(pendingUsers);
            ExecuteRemoteServersAction(server => _podsIntegrationService.RefreshUserCacheAsync(server, pendingUsers));
            _usersToRefreshService.DeRegisterPendingUsers(pendingUsers);
        }
    }
}
