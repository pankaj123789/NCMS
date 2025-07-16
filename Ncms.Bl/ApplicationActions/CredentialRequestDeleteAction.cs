using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestDeleteAction : CredentialRequestStateAction
    {

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.Draft, CredentialRequestStatusTypeName.ReadyForAssessment, CredentialRequestStatusTypeName.BeingAssessed };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.Deleted;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              CreateNote,
                                                              SetApplicationStatus,
                                                              SetExitState,
                                                          };

        protected override SecurityVerbName? RequiredSecurityVerb =>
            CredentialRequestModel.StatusTypeId == (int)CredentialRequestStatusTypeName.Draft
                ? SecurityVerbName.Update
                : SecurityVerbName.Delete;

    }
}
