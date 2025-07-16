using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestMarkAsSatAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestSitting;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.CheckedIn, CredentialRequestStatusTypeName.TestSessionAccepted };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestSat;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateTestSessionDate

                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              MarkTestSessionModelAsSat,
                                                              SetExitState,
                                                          };

        private void MarkTestSessionModelAsSat()
        {
            if (TestSessionModel != null)
            {
                TestSessionModel.Sat = true;
            }
        }
    }
}

