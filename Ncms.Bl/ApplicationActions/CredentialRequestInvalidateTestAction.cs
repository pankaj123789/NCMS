using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestInvalidateTestAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestSitting;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Invalidate;      

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestSat };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestInvalidated;

        protected override TestResultStatusTypeName RequiredTestResultStatus => TestResultStatusTypeName.TestInvalidated;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidateMinimumCredentialRequests,
            ValidateMandatoryFields,
            ValidateMandatoryPersonFields,
            ValidateMandatoryDocuments,
            ValidateTestResultStatus
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            SetApplicationStatus,
            SetExitState,
        };

        protected override void ValidateTestResultStatus()
        {
            if (TestSessionModel.TestResultId.HasValue)
            {
                base.ValidateTestResultStatus();
            }
        }
    }
}
