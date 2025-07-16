using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestCreatePaidTestReviewAction : CredentialRequestInvoiceBatchedAction
    {

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestFailed };

        protected override ProductSpecificationModel ActionFee => GetFee(FeeTypeName.PaidTestReview);


        protected override SystemActionTypeName InvoiceProcessedAction => SystemActionTypeName.PaidReviewInvoiceProcessed;
        protected override SystemActionTypeName InovicePaidAction => SystemActionTypeName.PaidReviewInvoicePaid;
        protected override CredentialRequestStatusTypeName ProcessingInvoiceStatus => CredentialRequestStatusTypeName.ProcessingPaidReviewInvoice;
        protected override CredentialRequestStatusTypeName PendingPaymentStatus => CredentialRequestStatusTypeName.AwaitingPaidReviewPayment;
        protected override CredentialRequestStatusTypeName InovicePaidStatus => CredentialRequestStatusTypeName.UnderPaidTestReview;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.PaidReview;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Create;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidateIfPaidReviewAllowed,
            CheckIfAlreadyHasPaidReview
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            CreateInvoiceIfApplicable,
            UnSetStandardSupplementaryComponentResults,
            UnSetRubricSupplementaryComponentResults,
            UpdateApplicationStatus,
            SetExitState
        };

        protected virtual void ValidateIfPaidReviewAllowed()
        {
            if (!AllowPaidReview)
            {
                throw new UserFriendlySamException(Naati.Resources.Test.PaidReviewNotAllowed);
            }
        }

        private void CheckIfAlreadyHasPaidReview()
        {
            if (TestSessionModel.TotalTestResults > 1)
            {
                throw new UserFriendlySamException(Naati.Resources.Test.TestAlreadyHasAPaidReview);
            }
        }

        protected override void UnSetRubricSupplementaryComponentResults()
        {
            if (CredentialRequestExitState == CredentialRequestStatusTypeName.UnderPaidTestReview)
            {
                base.UnSetRubricSupplementaryComponentResults();
            }
        }

        protected override void UnSetStandardSupplementaryComponentResults()
        {
            if (CredentialRequestExitState == CredentialRequestStatusTypeName.UnderPaidTestReview)
            {
                base.UnSetStandardSupplementaryComponentResults();
            }
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var sessionModel = GetTestSessionModel();
            var sessionResponse = TestSessionService.GetTestSessionById(sessionModel.TestSessionId);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), sessionResponse.Data.TestDate.ToString("dd MMMM yyyy"));
            base.GetEmailTokens(tokenDictionary);

        }

        protected override void CreateInvoiceIfApplicable()
        {
            if (IsSponsored || WizardModel.Source == SystemActionSource.MyNaati)
            {
                base.CreateInvoiceIfApplicable();
            }
        }

        protected override CredentialRequestStatusTypeName GetCredentialrequestExitState()
        {
            return (IsSponsored || WizardModel.Source == SystemActionSource.MyNaati) ? base.GetCredentialrequestExitState() : CredentialRequestStatusTypeName.UnderPaidTestReview;
        }
    }
}
