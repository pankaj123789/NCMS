using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using MyNaati.Contracts;
using MyNaati.Contracts.BackgroundTask;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.BackgroundTasks
{
    public class MyNaatiRefreshSystemCacheTask: BaseBackgroundTask, IMyNaatiRefreshSystemCacheTask
    {
        private readonly ILookupProvider _lookupProvider;
        private readonly IMyNaatiPodsIntegrationService _podsIntegrationService;
        private readonly IUtilityQueryService _utilityQueryService;

        public MyNaatiRefreshSystemCacheTask(ILookupProvider lookupProvider, IMyNaatiPodsIntegrationService podsIntegrationService,  ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _lookupProvider = lookupProvider;
            _podsIntegrationService = podsIntegrationService;
            _utilityQueryService = utilityQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            RefreshAllServersSystemCache();
        }

        public void RefreshLocalSystemCache()
        {
            var ip = _utilityQueryService.GetSystemIp();
            TaskLogger.WriteInfo("Refreshing all system cache on server {serverIp}", ip);
            _lookupProvider.RefreshAllCache();
        }

        private void RefreshAllServersSystemCache()
        {
            RefreshLocalSystemCache();
            ExecuteRemoteServersAction(_podsIntegrationService.RefreshSystemCacheAsync);
        }

     
    }
}
