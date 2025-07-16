using System.Collections.Generic;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    public class ProcessNcmsReportsBackgroundTask : BaseBackgroundTask, IProcessNcmsReportsBackgroundTask
    {
        private readonly IReportingQueryService _reportingQueryService;

        public ProcessNcmsReportsBackgroundTask(
            ISystemQueryService systemQueryService,
            IReportingQueryService reportingQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _reportingQueryService = reportingQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            ProcessExecuteNcmsReports();
        }

        private void ProcessExecuteNcmsReports()
        {
            TaskLogger.WriteInfo("Processing Executing NCMS Reports...");
            _reportingQueryService.ProcessExecuteNcmsReports();
            TaskLogger.WriteInfo("Finish Executing NCMS Reports.");
        }
    }
}