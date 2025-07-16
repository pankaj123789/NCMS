using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestMarkForAssessmentAction : CredentialRequestStateAction
    {
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Manage;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.ToBeIssued, CredentialRequestStatusTypeName.ProcessingRequest,  };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.ReadyForAssessment;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            SetOwner,
            CreateNote,
            SetExitState,
        };
    }
}
