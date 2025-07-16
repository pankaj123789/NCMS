using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using MyNaati.Contracts;
using MyNaati.Contracts.BackgroundTask;

namespace MyNaati.Bl.BackgroundTasks
{
    public class MyNaatiRefreshAllUsersTask : BaseBackgroundTask, IMyNaatiRefreshAllUsersTask
    {
        private readonly IMyNaatiPodsIntegrationService _podsIntegrationService;
        private readonly IUtilityQueryService _utilityQueryService;
        private readonly IDisplayBillsCacheQueryService _displayBillsCacheQueryService;
        private readonly INcmsUserCacheQueryService _ncmsUserCacheQueryService;
        private readonly INcmsUserPermissionQueryService _ncmsUserPermissionQueryService;
        private readonly IDisplayRolePlayerCacheQueryService _displayRolePlayerCacheQueryService;
        public MyNaatiRefreshAllUsersTask(
            ISystemQueryService systemQueryService,
            IMyNaatiPodsIntegrationService podsIntegrationService,
            IUtilityQueryService utilityQueryService,
            IBackgroundTaskLogger backgroundTaskLogger,
            IDisplayBillsCacheQueryService displayBillsCacheQueryService,
            INcmsUserCacheQueryService ncmsUserCacheQueryService,
            INcmsUserPermissionQueryService ncmsUserPermissionQueryService,
            IDisplayRolePlayerCacheQueryService displayRolePlayerCacheQueryService) : base(
            systemQueryService,
            backgroundTaskLogger,
            utilityQueryService)
        {
            _podsIntegrationService = podsIntegrationService;
            _utilityQueryService = utilityQueryService;
            _displayBillsCacheQueryService = displayBillsCacheQueryService;
            _ncmsUserCacheQueryService = ncmsUserCacheQueryService;
            _ncmsUserPermissionQueryService = ncmsUserPermissionQueryService;
            _displayRolePlayerCacheQueryService = displayRolePlayerCacheQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            RefreshAllServersUsersCache();
        }

        public void RefreshAllUsersLocalCache()
        {
            var ip = _utilityQueryService.GetSystemIp();
            TaskLogger.WriteInfo("Refreshing all users cache on server {serverIp}", ip);
            _displayBillsCacheQueryService.RefreshAllCache();
            _displayRolePlayerCacheQueryService.RefreshAllCache();
            _ncmsUserCacheQueryService.RefreshAllCache();
            _ncmsUserPermissionQueryService.RefreshAllCache();
        }

        private void RefreshAllServersUsersCache()
        {
            RefreshAllUsersLocalCache();
            ExecuteRemoteServersAction(_podsIntegrationService.RefreshAllUsersCacheAsync);
        }
    }
}
