using System;
using System.Collections.Generic;
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
    public class CandidateBriefBackgroundTask : ApplicationBackgroundTask, ICandidateBriefBackgroundTask
    {
        private readonly ITestMaterialQueryService _testMaterialQueryService;

        public CandidateBriefBackgroundTask(
            ISystemQueryService systemQueryService,
            ITestMaterialQueryService testMaterialQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _testMaterialQueryService = testMaterialQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            SendPendingCandidateBriefs();
        }

        private void SendPendingCandidateBriefs()
        {
            TaskLogger.WriteInfo("Getting Pending Candidate briefs to send...");
            var pendingCandidateBriefs = _testMaterialQueryService.GetPendingCandidateBriefsToSend(
                new PendingBriefRequest
                {
                    CredentialRequestStatus = CredentialRequestStatusTypeName.TestSessionAccepted,
                    SendDate = DateTime.Now.Date
                });

            TaskLogger.WriteInfo("Processing briefs...");
            ExecuteIfRunning(pendingCandidateBriefs, ExecuteSendCandidateBriefAction);
        }

        private void ExecuteSendCandidateBriefAction(ApplicationBriefDto applicationBrief)
        {
            ExecuteAction(SystemActionTypeName.SendCandidateBrief, applicationBrief);
        }

        private bool ExecuteAction(SystemActionTypeName action, ApplicationBriefDto pedingBrief)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}. ApplicationId: {ApplicationId}. CredentialRequestId: {CredentialRequestId}",
                action,
                pedingBrief.CredentialApplicationId,
                pedingBrief.CredentialRequestId);
            var applicationId = pedingBrief.CredentialApplicationId;
            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                CredentialRequestId = pedingBrief.CredentialRequestId,
                ActionType = (int)action
            };
            model.SetBackGroundAction(true);
            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                LogResponse(response, "CandidateBrief", applicationId);

                TaskLogger.AdProcessedApplication(
                    $"ApplicationId : {pedingBrief.CredentialApplicationId}, CredentialRequestId: {pedingBrief.CredentialRequestId}");

                return true;
            }

            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "CandidateBrief", string.Empty, true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "CandidateBrief", string.Empty, false);
            }

            return false;
        }
    }
}