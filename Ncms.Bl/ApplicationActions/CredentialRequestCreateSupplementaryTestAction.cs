using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestCreateSupplementaryTestAction : CredentialRequestInvoiceBatchedAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestFailed };

        protected override ProductSpecificationModel ActionFee => GetFee(FeeTypeName.SupplementaryTest);

        protected override TestResultStatusTypeName RequiredTestResultStatus => TestResultStatusTypeName.Failed;

        protected override SystemActionTypeName InvoiceProcessedAction => SystemActionTypeName.SupplementaryTestInvoiceProcessed;
        protected override SystemActionTypeName InovicePaidAction => SystemActionTypeName.SupplementaryTestInvoicePaid;
        protected override CredentialRequestStatusTypeName ProcessingInvoiceStatus => CredentialRequestStatusTypeName.ProcessingSupplementaryTestInvoice;
        protected override CredentialRequestStatusTypeName PendingPaymentStatus => CredentialRequestStatusTypeName.AwaitingSupplementaryTestPayment;
        protected override CredentialRequestStatusTypeName InovicePaidStatus => CredentialRequestStatusTypeName.TestAccepted;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.SupplementaryTest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Create;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateIfAllowsSupplementary,
            ValidateUserPermissions,
            ValidateTestResultStatus,
            ValidateStandardSupplementaryComponentResults,
            ValidateRubricSupplementaryComponentResults
        };

        protected void ValidateIfAllowsSupplementary()
        {
            if (!AllowSupplementary)
            {
                throw new UserFriendlySamException(Naati.Resources.Test.SupplementaryTestNotAllowed);
            }
        }

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            CreateInvoiceIfApplicable,
            MarkCredentialRequestAsSupplementary,
            UpdateApplicationStatus,
            SetExitState
        };

        private void MarkCredentialRequestAsSupplementary()
        {
            CredentialRequestModel.Supplementary = true;
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            GetStandardTestResultEmailTokens(tokenDictionary);
            GetRubricTestResultEmailTokens(tokenDictionary);
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
            return (IsSponsored || WizardModel.Source == SystemActionSource.MyNaati) ? base.GetCredentialrequestExitState() : CredentialRequestStatusTypeName.TestAccepted;
        }
    }
}
