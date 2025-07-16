using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.Models;

namespace Ncms.Bl.BackgroundTasks
{
    public abstract class ApplicationBackgroundTask : BaseBackgroundTask
    {
        protected ApplicationBackgroundTask(ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
        }

        protected void LogResponse(BusinessServiceResponse response, string type, int applicationId)
        {
            foreach (var message in response.Warnings)
            {
                TaskLogger.WriteApplicationWarning(message, applicationId, type, message, false);
            }

            foreach (var message in response.Errors)
            {
                TaskLogger.WriteApplicationWarning(message, applicationId, type, message, false);
            }
        }
    }
}