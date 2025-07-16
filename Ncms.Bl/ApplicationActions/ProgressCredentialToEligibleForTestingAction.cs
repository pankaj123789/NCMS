using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models;

namespace Ncms.Bl.ApplicationActions
{
    public class ProgressCredentialToEligibleForTestingAction : CredentialRequestStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.BeingChecked, CredentialApplicationStatusTypeName.Entered };
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.RequestEntered };

        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.InProgress;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                              UpdateCredentialRequestStatus,
                                                          };

        protected override void CreateNote()
        {
            //var credentialRequest = ApplicationModel.CredentialRequests.First();
            var credentialRequest = CredentialRequestModel;
            var credentialRequestType = credentialRequest.CredentialType.ExternalName;
            var skill = credentialRequest.Skill.DisplayName;
            var noteModel = new ApplicationNoteModel
            {
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CreatedDate = DateTime.Now,
                Note = $"{credentialRequestType} {skill} moved to Eligible for Testing because assessment is not required",
                UserId = CurrentUser.Id,
                ReadOnly = true
            };
            ApplicationModel.Notes.Add(noteModel);
        }

        protected override void SetExitState()
        {
            ApplicationModel.ApplicationInfo.ApplicationStatusTypeId = (int)ApplicationExitState;
            ApplicationModel.ApplicationInfo.StatusChangeUserId = CurrentUser.Id;
            ApplicationModel.ApplicationInfo.StatusChangeDate = DateTime.Now;
        }
        protected override void UpdateCredentialRequestStatus()
        {
            CredentialRequestModel.StatusTypeId = (int)CredentialRequestStatusTypeName.EligibleForTesting;
            CredentialRequestModel.StatusChangeUserId = CurrentUser.Id;
            CredentialRequestModel.StatusChangeDate = DateTime.Now;
        }

        protected override void CreatePendingEmailIfApplicable()
        {
            Output.PendingEmails = GetEmailPreviews();
        }
    }
}
