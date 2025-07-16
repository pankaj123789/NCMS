using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestUndoMarkAsSatAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestSitting;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestSat };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.CheckedIn;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateTestSessionDate,
                                                              ValidateTestStatus
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              SetOwner,
                                                              CreateNote,
                                                              UnMarkTestSessionModelAsSat,
                                                              SetExitState,
                                                          };


        private void ValidateTestStatus()
        {
            if (TestSessionModel.HasAssets || TestSessionModel.HasExaminers)
            {
                throw new UserFriendlySamException(Naati.Resources.Test.TestHasAssetsOrExaminers);
            }
        }

        private void UnMarkTestSessionModelAsSat()
        {
            if (TestSessionModel != null)
            {
                TestSessionModel.Sat = false;
            }
        }
    }
}