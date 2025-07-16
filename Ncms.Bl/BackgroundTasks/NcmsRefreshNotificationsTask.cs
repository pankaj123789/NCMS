using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
//using F1Solutions.Naati.Common.Contracts.Bl.Message;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    public class NcmsRefreshNotificationsTask : BaseBackgroundTask, INcmsRefreshNotificationsTask
    {
        //private readonly IMessenger _messenger;
        private readonly INotificationQueryService _notificationQueryService;
        private readonly INcmsPodsIntegrationService _ncmsPodsIntegrationService;
        private readonly IUtilityQueryService _utilityQueryService;

        public NcmsRefreshNotificationsTask(ISystemQueryService systemQueryService,
            IBackgroundTaskLogger backgroundTaskLogger,
            IUtilityQueryService utilityQueryService,
            //IMessenger messenger,
            INotificationQueryService notificationQueryService,
            INcmsPodsIntegrationService ncmsPodsIntegrationService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            //_messenger = messenger;
            _notificationQueryService = notificationQueryService;
            _ncmsPodsIntegrationService = ncmsPodsIntegrationService;
            _utilityQueryService = utilityQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            TaskLogger.WriteInfo("Deleting expired user notifications");
            var usersNames = _notificationQueryService.DeleteExpiredNotifications(DateTime.Now).Select(x => x.ToUserName).Distinct().ToList();

            var maxUsers = 10;
            var skipUsers = 0;
            do
            {
                var userNameList = usersNames.Skip(skipUsers).Take(maxUsers).ToList();
                RefreshLocalUserNotifications(userNameList);
                RefreshAllServersNotifications(userNameList);
                skipUsers += maxUsers;
            } while (skipUsers <= usersNames.Count);
        }

        private void RefreshAllServersNotifications(IEnumerable<string> userNames)
        {
            TaskLogger.WriteInfo($"Refreshing  remote users notifications {string.Join(",", userNames)} ");
            ExecuteRemoteServersAction(server => _ncmsPodsIntegrationService.RefreshUserNotifications(server, userNames));
        }

        public void RefreshLocalUserNotifications(IEnumerable<string> userNames)
        {
            var ip = _utilityQueryService.GetSystemIp();
            TaskLogger.WriteInfo("Refreshing all users notification on server {serverIp}. Users:{userNames}", ip, string.Join(",", userNames));
            //_messenger.Refresh(userNames);
        }
    }
}
