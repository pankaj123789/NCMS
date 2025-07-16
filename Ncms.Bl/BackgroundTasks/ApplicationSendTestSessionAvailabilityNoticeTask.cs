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
    public class ApplicationSendTestSessionAvailabilityNoticeTask : ApplicationBackgroundTask, IApplicationSendTestSessionAvailabilityNoticeTask
    {
        private readonly ITestSessionQueryService _testQuerySessionService;

        public ApplicationSendTestSessionAvailabilityNoticeTask(
            ISystemQueryService systemQueryService,
            ITestSessionQueryService testQuerySessionService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _testQuerySessionService = testQuerySessionService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
           SendTestSessions();
        }

        private void SendTestSessions()
        {
            HashSet<int> testSessionsIds = new HashSet<int>();

            TaskLogger.WriteInfo("Getting Test Session and Credential Request Ids for availability notice...");

            var testSessions = _testQuerySessionService.GetTestSessionAvailabilityObjects();

            TaskLogger.WriteInfo("Processing test sessions...");
            ExecuteIfRunning(testSessions,  ts => ExecuteSendApplicationTestSessionAction(ts, testSessionsIds));

            TaskLogger.WriteInfo("Setting AllowAvailabilityNotice...");
            if (testSessionsIds.Any())
            {
                _testQuerySessionService.DisableAllowAvailabilityNotice(testSessionsIds);
            }
            else
            {
                TaskLogger.WriteInfo("No AllowAvailabilityNotices to set...");
            }
        }

        private void ExecuteSendApplicationTestSessionAction(TestSessionAvailabilityObject testSessionAvailability, HashSet<int> testSessionsIds)
        {
            ExecuteAction(SystemActionTypeName.SendSessionAvailabilityNotice, testSessionAvailability, testSessionsIds);
        }

        private bool ExecuteAction(SystemActionTypeName action, TestSessionAvailabilityObject testSessionAvailability, HashSet<int> testSessionsIds)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}. TestSession: {TestSessionId} CredentialRequest: {CredentialRequest}. CredentialApplication: {CredentialApplication}",
                action,
                testSessionAvailability.TestSessionId,
                testSessionAvailability.CredentialRequestId, testSessionAvailability.ApplicationId);

            var model = new ApplicationActionWizardModel
            {
                CredentialRequestId = testSessionAvailability.CredentialRequestId,
                ActionType = (int)action,
                ApplicationId = testSessionAvailability.ApplicationId
            };
            model.SetBackGroundAction(true);
            model.SetNewTestSessionId(testSessionAvailability.TestSessionId);

            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                testSessionsIds.Add(testSessionAvailability.TestSessionId);
                LogResponse(response, "TestSession", testSessionAvailability.TestSessionId);

                //TaskLogger.AdProcessedApplication($"ApplicationId : {testSessionAvailability.ApplicationId}");

                return true;
            }

            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteWarning("Exception message: " + ex.Message + " Test Session Id: " + testSessionAvailability.TestSessionId);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteError(ex, "undefined", "Test Session Availability notice", false);
            }

            return false;
        }
    }
}