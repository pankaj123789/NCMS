using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    public class NcmsRefreshSystemCacheTask : BaseBackgroundTask, INcmsRefreshSystemCacheTask
    {
       
        private readonly ILookupTypeConverterHelper _lookupTypeConverterHelper;
        private readonly INcmsPodsIntegrationService _ncmsPodsIntegrationService;

        public NcmsRefreshSystemCacheTask(
            ISystemQueryService systemQueryService,
            ILookupTypeConverterHelper lookupTypeConverterHelper,
            IBackgroundTaskLogger backgroundTaskLogger,
            INcmsPodsIntegrationService ncmsPodsIntegrationService, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _lookupTypeConverterHelper = lookupTypeConverterHelper;
            _ncmsPodsIntegrationService = ncmsPodsIntegrationService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            RefreshAllServersSystemCache();
        }

        public void RefreshLocalSystemCache()
        {
            TaskLogger.WriteInfo("Refreshing System Cache ...");
            _lookupTypeConverterHelper.RefreshAllCache();
        }

        private void RefreshAllServersSystemCache()
        {
            RefreshLocalSystemCache();
            ExecuteRemoteServersAction(_ncmsPodsIntegrationService.RefreshSystemCacheAsync);
        }
    }
}
