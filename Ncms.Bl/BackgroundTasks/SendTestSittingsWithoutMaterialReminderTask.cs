using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.TestSessionActions;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.BackgroundTasks
{
    public class SendTestSittingsWithoutMaterialReminderTask : ApplicationBackgroundTask, ISendTestSittingsWithoutMaterialReminderTask
    {
        private readonly ITestSpecificationQueryService _testSpecificationQueryService;

        public SendTestSittingsWithoutMaterialReminderTask(
            ISystemQueryService systemQueryService,
            ITestSpecificationQueryService testSpecificationQueryService, 
            IBackgroundTaskLogger backgroundTaskLogger, 
            IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _testSpecificationQueryService = testSpecificationQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            SendTestSittingsWithoutMaterialReminders();
        }

        private void SendTestSittingsWithoutMaterialReminders()
        {
            var sendTestSessionSittingMaterialReminderAction = new SendTestSessionSittingMaterialReminderAction();
            sendTestSessionSittingMaterialReminderAction.Perform(); 
        }
    }
}