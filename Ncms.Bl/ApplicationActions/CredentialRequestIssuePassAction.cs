using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestIssuePassAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestSat };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.IssuedPassResult;
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestResult;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Issue;

        protected override TestResultStatusTypeName RequiredTestResultStatus => TestResultStatusTypeName.Passed;
        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidateMinimumCredentialRequests,
            ValidateMandatoryFields,
            ValidateMandatoryPersonFields,
            ValidateMandatoryDocuments,
            ValidateAllowIssue,
            ValidateTestResultStatus,
            ValidateStandardMarks
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            SetExitState
        };
     

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            GetStandardTestResultEmailTokens(tokenDictionary);
            GetRubricTestResultEmailTokens(tokenDictionary);
        }
    }
}
