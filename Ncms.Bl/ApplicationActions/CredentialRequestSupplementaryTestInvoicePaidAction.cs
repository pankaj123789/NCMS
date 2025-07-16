using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestSupplementaryTestInvoicePaidAction : CredentialRequestTestInvoicePaidAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.PersonFinanceDetails;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Manage;
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] {  CredentialRequestStatusTypeName.AwaitingSupplementaryTestPayment };
        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateStandardSupplementaryComponentResults,
            ValidateRubricSupplementaryComponentResults,
            ValidateCredentialRequestInvoices
        };

        protected override CredentialRequestTestSessionModel GetTestSessionModel()
        {
            return CredentialRequestModel.TestSessions.OrderByDescending(x => x.CredentialTestSessionId).FirstOrDefault(x => !x.Rejected && x.Supplementary == false);
        }

        protected override CredentialWorkflowFeeModel GetWorkflowFee()
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

            return fees.SingleOrDefault();
        }
    }
}
