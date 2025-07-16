using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestRefundCreditNotePaidAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.AwaitingCreditNotePayment };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => GetExitState();

        protected override SecurityNounName? RequiredSecurityNoun => GetRequiredSecurityNoun();
        protected override SecurityVerbName? RequiredSecurityVerb => GetRequiredSecurityVerb();

        protected override IList<Action> Preconditions => new List<Action>
                                                            {
                                                                ValidateEntryState,
                                                                ValidateUserPermissions,
                                                                ValidateRefundCreditNote
                                                            };

        protected override IList<Action> SystemActions => new List<Action>
                                                            {
                                                                ClearOwner,
                                                                CreateNote,
                                                                SetApplicationStatus,
                                                                SetExitState,
                                                                ProcessRefundRequest,
                                                            };

        protected virtual SecurityNounName GetRequiredSecurityNoun()
        {
            return SecurityNounName.PersonFinanceDetails;
        }

        protected virtual SecurityVerbName GetRequiredSecurityVerb()
        {
            return SecurityVerbName.Manage;
        }

        private CredentialRequestStatusTypeName GetExitState()
        {
            return CredentialRequestStatusTypeName.Withdrawn;
        }

        protected override void ProcessRefundRequest()
        {
            base.ProcessRefundRequest();

            var refundRequest = RefundRequest;
            refundRequest.CreditNotePaymentProcessedDate = DateTime.Now;
        }

        protected virtual void ValidateRefundCreditNote()
        {
            if (RefundRequest == null)
            {
                throw new ArgumentNullException(nameof(RefundRequest));
            }

            var creditNoteResponse = FinanceService.GetInvoices(new GetInvoicesRequest
            {
                InvoiceNumber = new[] { RefundRequest.CreditNoteNumber }
            });

            if (!string.IsNullOrWhiteSpace(creditNoteResponse.WarningMessage))
            {
                ValidationErrors.Add(new ValidationResultModel
                {
                    Message = creditNoteResponse.WarningMessage
                });
            }

            if (!string.IsNullOrWhiteSpace(creditNoteResponse.ErrorMessage))
            {
                ValidationErrors.Add(new ValidationResultModel
                {
                    Message = creditNoteResponse.ErrorMessage
                });
            }

            if (ValidationErrors.Any())
            {
                return;
            }

            var notPaidCreditNote = creditNoteResponse.Invoices.Where(x => x.Balance != 0 && x.Status != InvoiceStatus.Canceled ).ToList();

            foreach (var invoice in notPaidCreditNote)
            {
                ValidationErrors.Add(new ValidationResultModel
                {
                    Message = string.Format(Naati.Resources.Application.UnpaidApplicationInvoiceErrorMessage, invoice.InvoiceNumber)
                });
            }
        }
    }
}
