using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationDeleteAction : ApplicationStateAction
    {
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.Draft };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.Deleted;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions

                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                              UpdateCredentialRequestStatus
                                                          };

        protected override void UpdateCredentialRequestStatus()
        {
            if (ApplicationType.RequiresAssessment)
            {
                foreach (var credentialRequest in ApplicationModel.CredentialRequests)
                {
                    credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.Deleted;
                    credentialRequest.StatusChangeUserId = CurrentUser.Id;
                    credentialRequest.StatusChangeDate = DateTime.Now;
                }
            }
        }
    }
}
