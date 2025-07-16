using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestRejectTestSessionAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestSession;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Reject;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates
            => new[] { CredentialRequestStatusTypeName.TestSessionAccepted, CredentialRequestStatusTypeName.TestSat, CredentialRequestStatusTypeName.CheckedIn};

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestAccepted;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateTestSessionRejectionDate

                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              RejectTestSession,
                                                              SetExitState,
                                                          };
       

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var sessionModel = TestSessionModel;
            if (sessionModel == null)
            {
                throw new Exception("Reject Test Session: can't determine which session is being rejected.");
            }

            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), sessionModel.TestDate.ToString("dd MMMM yyyy"));
        }
    }
}
