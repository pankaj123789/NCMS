using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestWithdrawPaidReviewAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AwaitingPaidReviewPayment, CredentialRequestStatusTypeName.UnderPaidTestReview };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestFailed;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.PaidReview;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Withdraw;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidatePaidReviewExaminersNotAdded
        };
        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            SetApplicationStatus,
            RemoveFee,
            SetExitState,
        };

        private void ValidatePaidReviewExaminersNotAdded()
        {
            var response = ApplicationService.CanWithdrawApplicationUnderPaidReview(WizardModel.CredentialRequestId);

            if (!response)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.PaidReviewExaminersAddedErrorMessage);
            }
        }

        private void RemoveFee()
        {
            var fee = CredentialRequestModel.CredentialWorkflowFees
                .Where(x => x.OnPaymentActionType == SystemActionTypeName.PaidReviewInvoicePaid
                            && x.PaymentActionProcessedDate == null)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
           
            if (fee != null)
            {
                ApplicationService.RemoveFee(fee.Id);
            }
        }
    }
}
