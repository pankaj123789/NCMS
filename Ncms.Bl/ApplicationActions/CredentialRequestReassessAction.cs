using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestReassessAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AssessmentFailed };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.ReadyForAssessment;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              CreateNote,
                                                              ReactivateApplication,
                                                              SetExitState,
                                                          };

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Manage;

        protected virtual void ReactivateApplication()
        {
            if (ApplicationModel.ApplicationStatus.Id == (int)CredentialApplicationStatusTypeName.Completed)
            {
                var action = CreateAction(SystemActionTypeName.ReactivateApplication, ApplicationModel,
                    WizardModel);
                action.Perform();
            }
        }
    }
}
