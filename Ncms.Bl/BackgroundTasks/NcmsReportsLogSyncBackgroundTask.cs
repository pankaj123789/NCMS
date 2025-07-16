using System.Collections.Generic;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    public class NcmsReportsLogSyncBackgroundTask : BaseBackgroundTask, INcmsReportsLogSyncBackgroundTask
    {
        private readonly IReportingQueryService _reportingQueryService;

        public NcmsReportsLogSyncBackgroundTask(
            ISystemQueryService systemQueryService,
            IReportingQueryService reportingQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _reportingQueryService = reportingQueryService;
        }

        public override void Execute(IDictionary<string,string> parameters)
        {
            ProcessSyncReportLogs();
        }

        private void ProcessSyncReportLogs()
        {
            TaskLogger.WriteInfo("Processing Sync Report Logs...");
            _reportingQueryService.ClearNcmsReportingCache();
            ThrowIfNotRunning();
            _reportingQueryService.ProcessSyncReportLogs();
            ThrowIfNotRunning();
            TaskLogger.WriteInfo("Finish Executing Sync Report Logs...");
        }
    }
}