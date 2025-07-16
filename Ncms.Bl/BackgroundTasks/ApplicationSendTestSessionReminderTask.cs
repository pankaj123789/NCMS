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
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.BackgroundTasks
{
    public class ApplicationSendTestSessionReminderTask : ApplicationBackgroundTask, IApplicationSendTestSessionReminderTask
    {
        private readonly IApplicationQueryService _applicationQueryService;

        public ApplicationSendTestSessionReminderTask(
            ISystemQueryService systemQueryService,
            IApplicationQueryService applicationQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _applicationQueryService = applicationQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            SendTestSessionReminders();
        }

        private void SendTestSessionReminders()
        {
            TaskLogger.WriteInfo("Getting Application Ids with eligible test session reminders...");

            List<DateTime> sessionDates = new List<DateTime>();
            var timePeriods = GetSystemValue("SendTestSessionReminderTimePeriodsDays").Split(',');
            foreach (var timePeriod in timePeriods)
            {
                sessionDates.Add(DateTime.Today.AddDays(Convert.ToInt32(timePeriod)));
            }

            var testSessions = _applicationQueryService.GetTestSessionReminders(
                new GetTestSessionRemindersRequest
                {
                    TestSessionDates = sessionDates
                });

            TaskLogger.WriteInfo("Processing reminders...");
            ExecuteIfRunning(testSessions, ExecuteSendApplicationTestSessionReminderAction);
        }

        private void ExecuteSendApplicationTestSessionReminderAction(TestSessionReminderObject testSession)
        {
            ExecuteAction(SystemActionTypeName.SendTestSessionReminder, testSession);
        }

        private bool ExecuteAction(SystemActionTypeName action, TestSessionReminderObject testSession)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}. ApplicationId: {ApplicationId} CredentialRequest: {CredentialRequest}.",
                action,
                testSession.ApplicationId,
                testSession.CredentialRequestId);

            var model = new ApplicationActionWizardModel
            {
                ApplicationId = testSession.ApplicationId,
                CredentialRequestId = testSession.CredentialRequestId,
                ActionType = (int) action
            };
            model.SetBackGroundAction(true);
            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                LogResponse(response, "TestSessionReminder", testSession.ApplicationId);

                TaskLogger.AdProcessedApplication($"ApplicationId : {testSession.ApplicationId}");

                return true;
            }

            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationWarning("Exception message: " + ex.Message, testSession.ApplicationId, "Test Session Reminder", string.Empty, true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, testSession.ApplicationId, "TestSession Reminder", string.Empty, false);
            }

            return false;
        }
    }
}