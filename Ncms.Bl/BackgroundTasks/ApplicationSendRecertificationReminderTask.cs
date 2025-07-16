using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.BackgroundTasks
{
    public class ApplicationSendRecertificationReminderTask : ApplicationBackgroundTask, IApplicationSendRecertificationReminderTask
    {
        private readonly IApplicationQueryService _applicationQueryService;

        public ApplicationSendRecertificationReminderTask(
            ISystemQueryService systemQueryService,
            IApplicationQueryService applicationQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _applicationQueryService = applicationQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            SendRecertificationReminders();
        }

        private void SendRecertificationReminders()
        {
            TaskLogger.WriteInfo("Getting Application Ids with eligible recertification reminders...");

            List<DateTime> expiryDates = new List<DateTime>();
            var timePeriods = GetSystemValue("SendRecertificationReminderTimePeriodsDays").Split(',');
            foreach (var timePeriod in timePeriods)
            {
                expiryDates.Add(DateTime.Today.AddDays(Convert.ToInt32(timePeriod)));
            }

            //change to objects
            var applicationIds = _applicationQueryService.GetApplicationIdsWithRecertificationReminders(
                new GetApplicationIdsWithRecertificationRemindersRequest
                {
                    ExpiryDates = expiryDates
                });

            TaskLogger.WriteInfo("Processing reminders...");
            ExecuteIfRunning(applicationIds, ExecuteSendApplicationRecertificationReminderAction);
        }

        private void ExecuteSendApplicationRecertificationReminderAction(int applicationId)
        {
            ExecuteAction(SystemActionTypeName.SendRecertificationReminder, applicationId);
        }

        private bool ExecuteAction(SystemActionTypeName action, int applicationId)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}. ApplicationId: {ApplicationId}.",
                action,
                applicationId);

            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                CredentialRequestId = 0,
                ActionType = (int) action
            };
            model.SetBackGroundAction(true);
            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                LogResponse(response, "RecertificationReminder", applicationId);

                TaskLogger.AdProcessedApplication($"ApplicationId : {applicationId}");

                return true;
            }

            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationWarning("Exception message: " + ex.Message, applicationId, "Recertification Reminder", string.Empty, true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "Recertification Reminder", string.Empty, false);
            }

            return false;
        }
    }

    public class ApplicationRecertificationReminderObject
    {
        private int ApplicationId { get; set; }
    }
}