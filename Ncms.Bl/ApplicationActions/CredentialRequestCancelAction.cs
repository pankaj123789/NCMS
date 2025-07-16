using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestCancelAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.RequestEntered };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.Cancelled;

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

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Cancel;
    }
}
