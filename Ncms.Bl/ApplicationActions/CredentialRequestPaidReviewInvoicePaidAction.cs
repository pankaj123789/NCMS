using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestPaidReviewInvoicePaidAction : CredentialRequestStateAction
    {

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.PersonFinanceDetails;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Manage;
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AwaitingPaidReviewPayment };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.UnderPaidTestReview;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateCredentialRequestInvoices
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            UnSetStandardSupplementaryComponentResults,
            UnSetRubricSupplementaryComponentResults,
            SetExitState,
            ProcessFee,
        };


        protected override CredentialWorkflowFeeModel GetWorkflowFee()
        {
            var fee = CredentialRequestModel.CredentialWorkflowFees
                .Where(x => x.OnPaymentActionType == SystemActionTypeName.PaidReviewInvoicePaid
                            && x.PaymentActionProcessedDate == null)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
          

            if (fee == null)
            {
                throw new UserFriendlySamException(
                    $"No fees found for {CredentialRequestModel.CredentialName} - {CredentialRequestModel.Skill.DisplayName} on APP{ApplicationModel.ApplicationInfo.ApplicationId}.");
            }

            return fee;
        }
    }
}
