using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestRequestRevertResultsAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.IssuedPassResult, CredentialRequestStatusTypeName.CertificationIssued, CredentialRequestStatusTypeName.TestFailed, CredentialRequestStatusTypeName.TestInvalidated };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => GetExitState();

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestResult;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Revert;


        protected SystemActionEventTypeName ActionEvent { get; set; }

        protected override void ConfigureInstance(CredentialApplicationDetailedModel application, ApplicationActionWizardModel wizardModel,
            ApplicationActionOutput output)
        {
            base.ConfigureInstance(application, wizardModel, output);
            DefineActionEvent();
        }

        protected override IList<Action> Preconditions => new List<Action>
        {

            ValidateEntryState,
            ValidateTestSitting,
            ValidateCredentialType,
            ValidateUserPermissions,
            };

        protected override IList<Action> SystemActions => new List<Action>
        {
            CreateNote,
            SetApplicationStatus,
            SetExitState
        };

        private void DefineActionEvent()
        {
            switch (CredentialRequestModel.StatusTypeId)
            {
                case (int)CredentialRequestStatusTypeName.TestFailed:
                    ActionEvent = SystemActionEventTypeName.FailedTestReverted;
                    break;
                case (int)CredentialRequestStatusTypeName.IssuedPassResult:
                    ActionEvent = SystemActionEventTypeName.PassedTestReverted;
                    break;
                case (int)CredentialRequestStatusTypeName.CertificationIssued:
                    ActionEvent = SystemActionEventTypeName.IssuedCredentialReverted;
                    break;
                case (int)CredentialRequestStatusTypeName.TestInvalidated:
                    ActionEvent = SystemActionEventTypeName.InvalidatedTestReverted;
                    break;
                default:
                    ActionEvent = SystemActionEventTypeName.None;
                    break;
            }
        }

        private void ValidateTestSitting()
        {
            if (TestSessionModel == null)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.TestSittingNotFound);
            }
        }
        private void ValidateCredentialType()
        {
            if (CredentialRequestModel.CredentialType.Certification && TestSessionModel.TestResultStatusId == (int)TestResultStatusTypeName.Passed)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.ActionNotSupportedForCertification);
            }
        }

        private CredentialRequestStatusTypeName GetExitState()
        {
            if (TestSessionModel.HasPaidReviewExaminers)
            {
                return CredentialRequestStatusTypeName.UnderPaidTestReview;
            }

            return CredentialRequestStatusTypeName.TestSat;
        }

        protected override void SetApplicationStatus()
        {
            if (ApplicationModel.ApplicationStatus.Id == (int)CredentialApplicationStatusTypeName.Completed)
            {
                var action = CreateAction(SystemActionTypeName.ReactivateApplication, ApplicationModel, WizardModel);
                action.Perform();
            }
        }

        protected override GenericResponse<UpsertApplicationResultModel> SaveActionData()
        {
            var credential = CredentialRequestModel.Credentials.FirstOrDefault();
            if (credential != null)
            {
                try
                {

                    ApplicationService.RollbackIssueCredential(new RollbackIssueCredentialModel
                    {
                        ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                        CredentialRequestId = CredentialRequestModel.Id,
                        ApplicationOriginalStatusId = ApplicationModel.ApplicationStatus.Id,
                        ApplicationOriginalStatusDate = ApplicationModel.ApplicationInfo.StatusChangeDate,
                        ApplicationOriginalStatusUserId = ApplicationModel.ApplicationInfo.StatusChangeUserId,
                        CredentialRequestOriginalStatusId = CredentialRequestModel.StatusTypeId,
                        CredentialRequestOriginalStatusDate = CredentialRequestModel.StatusChangeDate,
                        CredentialRequestOriginalStatusUserId = CredentialRequestModel.StatusChangeUserId,
                        Credential = null,
                        StoredFileIds = credential.StoredFileIds,
                    });

                }
                catch (Exception ex)
                {
                    LoggingHelper.LogError("APP{ApplicationId} CR{CredentialRequestId} Revert  Credential action rolled back due to exception", ApplicationModel.ApplicationInfo.ApplicationId, CredentialRequestModel.Id);
                    throw new Exception($"While trying to revert Credential action for APP{ApplicationModel.ApplicationInfo.ApplicationId}/CR{CredentialRequestModel.Id} an error ocurred.", ex);
                }

                CredentialRequestModel.Credentials.Remove(credential);
            }

            return base.SaveActionData();
        }

        protected override IList<ActionEventLevel> GetActionEvents()
        {
            var events = base.GetActionEvents();
            var maxLevel = events.Max(x => x.Level);
            var actionEvent = new ActionEventLevel { Event = ActionEvent, Level = maxLevel + 1 };
            events.Add(actionEvent);
            return events;
        }
    }
}
