using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestUndoCheckInAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestSitting;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.CheckedIn };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestSessionAccepted;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateTestSessionDate
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              SetOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                          };
    }
}