using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestWithdrawSupplementaryTestAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AwaitingSupplementaryTestPayment, CredentialRequestStatusTypeName.TestAccepted };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestFailed;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.SupplementaryTest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Withdraw;

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
            UnMarkCredentialRequestAsSupplementary,
            RemoveFee,
            SetExitState,
        };

        private void UnMarkCredentialRequestAsSupplementary()
        {
            CredentialRequestModel.Supplementary = false;
        }

        private void RemoveFee()
        {
            var fees = CredentialRequestModel.CredentialWorkflowFees
                .Where(x => x.OnPaymentActionType == SystemActionTypeName.SupplementaryTestInvoicePaid
                            && x.PaymentActionProcessedDate == null)
                .ToList();

            if (fees.Count > 1)
            {
                throw new UserFriendlySamException(
                    $"{fees.Count} fees found for {CredentialRequestModel.CredentialName} - {CredentialRequestModel.Skill.DisplayName} on APP{ApplicationModel.ApplicationInfo.ApplicationId}. Expecting only 1.");
            }

            var fee = fees.SingleOrDefault();
            if (fee != null)
            {
                ApplicationService.RemoveFee(fee.Id);
            }
        }
    }
}
