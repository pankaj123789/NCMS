using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestWithdrawAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] {
            CredentialRequestStatusTypeName.EligibleForTesting,
            CredentialRequestStatusTypeName.AwaitingTestPayment,
            CredentialRequestStatusTypeName.TestAccepted,
            CredentialRequestStatusTypeName.TestSessionAccepted,
            CredentialRequestStatusTypeName.ReadyForAssessment,
            CredentialRequestStatusTypeName.BeingAssessed,
            CredentialRequestStatusTypeName.TestSat,
            CredentialRequestStatusTypeName.CheckedIn,
        };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.Withdrawn;

        protected override SecurityNounName? RequiredSecurityNoun => GetRequiredSecurityNoun();
        protected override SecurityVerbName? RequiredSecurityVerb => GetRequiredSecurityVerb();

        protected override IList<Action> Preconditions => new List<Action>
                                                                        {
                                                                            ValidateEntryState,
                                                                            ValidateUserPermissions
                                                                        };

        protected override IList<Action> SystemActions => new List<Action>
                                                                        {
                                                                            ClearOwner,
                                                                            CreateNote,
                                                                            SetApplicationStatus,
                                                                            RejectTestSession,
                                                                            SetExitState,
                                                                         };

        protected virtual SecurityNounName GetRequiredSecurityNoun()
        {
            if (!IsInvoicedStatus((CredentialRequestStatusTypeName)CredentialRequestModel.StatusTypeId))
            {
                return SecurityNounName.CredentialRequest;
            }

            return SecurityNounName.FinanceOther;
        }

        private bool IsInvoicedStatus(CredentialRequestStatusTypeName status)
        {
            return (status != CredentialRequestStatusTypeName.EligibleForTesting) &&
                   (status != CredentialRequestStatusTypeName.ReadyForAssessment) &&
                   (status != CredentialRequestStatusTypeName.BeingAssessed);
        }

        protected virtual SecurityVerbName GetRequiredSecurityVerb()
        {
            if (!IsInvoicedStatus((CredentialRequestStatusTypeName)CredentialRequestModel.StatusTypeId))
            {
                return SecurityVerbName.Withdraw;
            }
            return SecurityVerbName.Manage;
        }
    }
}
