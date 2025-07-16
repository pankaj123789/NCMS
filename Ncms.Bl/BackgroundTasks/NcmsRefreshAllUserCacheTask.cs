using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    public class NcmsRefreshAllUserCacheTask : BaseBackgroundTask, INcmsRefreshAllUserCacheTask
    {
        private readonly IUtilityQueryService _utilityQueryService;
        private readonly INcmsUserCacheQueryService _ncmsUserCacheQueryService;
        private readonly INcmsUserPermissionQueryService _ncmsUserPermissionCacheQueryService;
        private readonly INcmsPodsIntegrationService _ncmsPodsIntegrationService;

        public NcmsRefreshAllUserCacheTask(
            ISystemQueryService systemQueryService,
            IBackgroundTaskLogger backgroundTaskLogger,
            INcmsUserCacheQueryService ncmsUserCacheQueryService,
            INcmsUserPermissionQueryService ncmsUserPermissionCacheQueryService,
            INcmsPodsIntegrationService ncmsPodsIntegrationService, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _utilityQueryService = utilityQueryService;
            _ncmsUserCacheQueryService = ncmsUserCacheQueryService;
            _ncmsUserPermissionCacheQueryService = ncmsUserPermissionCacheQueryService;
            _ncmsPodsIntegrationService = ncmsPodsIntegrationService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            RefreshAllServersUsersCache();
        }

        public void RefreshAllUsersLocalCache()
        {
            var ip = _utilityQueryService.GetSystemIp();
            TaskLogger.WriteInfo("Refreshing all users cache on server {serverIp}", ip);
            _ncmsUserCacheQueryService.RefreshAllCache();
            _ncmsUserPermissionCacheQueryService.RefreshAllCache();
        }

        private void RefreshAllServersUsersCache()
        {
            RefreshAllUsersLocalCache();
            ExecuteRemoteServersAction(_ncmsPodsIntegrationService.RefreshAllUsersCacheAsync);
        }
    }
}