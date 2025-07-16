using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
//using F1Solutions.Naati.Common.Contracts.Bl.Message;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;

namespace Ncms.Bl.BackgroundTasks
{
    public class NcmsRefreshPendingUsersTask : BaseBackgroundTask, INcmsRefreshPendingUsersTask
    {
        private readonly IUtilityQueryService _utilityQueryService;
        private readonly INcmsUserRefreshCacheQueryService _userRefreshCacheQueryService;
        //private readonly IMessenger _messenger;
        private readonly INcmsUserCacheQueryService _ncmsUserCacheQueryService;
        private readonly INcmsUserPermissionQueryService _ncmsUserPermissionCacheQueryService;
        private readonly ICookieQueryService _cookieQueryService;
        private readonly INcmsPodsIntegrationService _ncmsPodsIntegrationService;

        public NcmsRefreshPendingUsersTask(
            ISystemQueryService systemQueryService,
            IBackgroundTaskLogger backgroundTaskLogger,
            INcmsUserCacheQueryService ncmsUserCacheQueryService,
            INcmsUserPermissionQueryService ncmsUserPermissionCacheQueryService,
            ICookieQueryService cookieQueryService,
            INcmsPodsIntegrationService ncmsPodsIntegrationService, IUtilityQueryService utilityQueryService,
            //IMessenger messenger,
            INcmsUserRefreshCacheQueryService userRefreshCacheQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _utilityQueryService = utilityQueryService;
            _userRefreshCacheQueryService = userRefreshCacheQueryService;
            //_messenger = messenger;
            _ncmsUserCacheQueryService = ncmsUserCacheQueryService;
            _ncmsUserPermissionCacheQueryService = ncmsUserPermissionCacheQueryService;
            _cookieQueryService = cookieQueryService;
            _ncmsPodsIntegrationService = ncmsPodsIntegrationService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue(BackgroundTasksParameters.ServerName, value: out var serverName))
            {
                serverName = _utilityQueryService.GetSystemIp();
            }

            RefreshAllServersPendingUsers(serverName);
        }

        public void RefreshLocalUsers(IEnumerable<NcmsUserRefreshDto> users)
        {
            var ip = _utilityQueryService.GetSystemIp();

            var refreshCacheUsers = users.Where(x => x.RefreshTypes.Any(t => t == UserRefreshType.Cache)).ToList();
            var refreshNotificationUsers = users.Where(x => x.RefreshTypes.Any(t => t == UserRefreshType.Notification)).ToList();

            foreach (var user in refreshCacheUsers)
            {
                TaskLogger.WriteInfo("Refreshing user cache for user {cacheUserName} on server {serverIp}", user.Identifier, ip);
                _ncmsUserCacheQueryService.RefreshUserCache(user.Identifier);
                _ncmsUserPermissionCacheQueryService.RefreshUserCache(user.Identifier);
                foreach (var invalidCookie in user.InvalidCookies)
                {
                    _cookieQueryService.InvalidateCacheCookie(invalidCookie);}
            }

            //TaskLogger.WriteInfo("Refreshing users notifications on server {serverIp}", ip);
            //var notificationIds = refreshNotificationUsers.SelectMany(x => x.NotificationIds).ToList();
            //_messenger.Notify(notificationIds);
        }

        public int RegisterUserCacheRefresh(string userName, string invalidCookie, DateTime cookieExpiryDate)
        {
            TaskLogger.WriteInfo($"Refreshing NCMS database server cookie for user {userName}...");
            // Add Cookie to db, so new server will be able to get existing invalid cookies
            _cookieQueryService.InvalidateDbCookie(invalidCookie, cookieExpiryDate);

            var jobDelay = _userRefreshCacheQueryService.RegisterUserCacheRefreshJob(userName, invalidCookie);

            return jobDelay;
        }

        public int RegisterHubNotification(string userName, int notificationId)
        {
            TaskLogger.WriteInfo($"Registering NCMS notification hub for user {userName}...");

            var jobDelay = _userRefreshCacheQueryService.RegisterUserHubNotificationJob(userName, notificationId);

            return jobDelay;
        }

        private void RefreshAllServersPendingUsers(string sourceServerName)
        {
            var localIp = _utilityQueryService.GetSystemIp();

            if (sourceServerName != localIp)
            {
                if (GetRemoteHangFireServers().All(x => x != sourceServerName))
                {
                    TaskLogger.WriteWarning($"Job {nameof(RefreshAllServersPendingUsers)} will not be executed because source server {sourceServerName} was not found.");
                    return;
                }
                var callName = nameof(_ncmsPodsIntegrationService.ExecuteRefreshPendingUserTaskAsync);
                TaskLogger.WriteInfo($"Redirecting job {callName} to server {sourceServerName}");
                var task = _ncmsPodsIntegrationService.ExecuteRefreshPendingUserTaskAsync(sourceServerName);
                var completed = task.Wait(60000);

                TaskLogger.WriteInfo($"Redirect of job {callName} to server {sourceServerName} finished with completed status: {completed}. Errors: {!task.IsCompleted || task.Exception != null}");
                return;
            }

            var pendingUsers = _userRefreshCacheQueryService.GetUsersToRefresh();
            RefreshLocalUsers(pendingUsers);
            ExecuteRemoteServersAction(server => _ncmsPodsIntegrationService.RefreshUserCacheAsync(server, pendingUsers));
            _userRefreshCacheQueryService.DeRegisterPendingUsers(pendingUsers);
        }
    }
}