using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Accounting;
using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public abstract class CredentialRequestCreditNoteBatchedAction : CredentialRequestStateAction
    {
        public const string PAYPALTRANSPREFIX = "PAYPAL";
        protected abstract bool HasCreditCardPayment { get; }
        protected abstract bool HasPayPalPayment { get; }
        protected abstract SystemActionTypeName CreditNoteProcessedAction { get; }
        protected abstract SystemActionTypeName CreditNotePaidAction { get; }
        protected abstract CredentialRequestStatusTypeName ProcessingCreditNoteStatus { get; }
        protected abstract CredentialRequestStatusTypeName PendingPaymentStatus { get; }
        protected abstract CredentialRequestStatusTypeName CreditNotePaidStatus { get; }
        protected override CredentialRequestStatusTypeName CredentialRequestExitState => GetCredentialRequestExitState();
        protected override SystemActionTypeName? OnCreditNoteCreationActionType => GetOnCreditNoteCreationActionType();
        protected override SystemActionTypeName? OnCreditNotePaymentActionType => GetOnCreditNotePaymentActionType();

        protected virtual SystemActionTypeName? GetOnCreditNoteCreationActionType()
        {
            if (ProcessInBatch && !(HasCreditCardPayment || HasPayPalPayment))
            {
                return CreditNoteProcessedAction;
            }
            return null;
        }

        protected virtual SystemActionTypeName? GetOnCreditNotePaymentActionType()
        {
            if (ProcessInBatch && (HasCreditCardPayment || HasPayPalPayment))
            {
                return CreditNoteProcessedAction;
            }

            return !HasCreditCardPayment ? CreditNotePaidAction: (SystemActionTypeName?)null;  
        }

        protected virtual InvoicePaymentCreateModel GetPayment()
        {
            EftMachineModel eftMachine=null;
            if (!(HasCreditCardPayment||HasPayPalPayment))
            {
                throw new Exception("Payment details need to be specified.");
            }

            var paymentAccount = new Guid(SystemService.GetSystemValue("WiisePaymentAccount"));
            if (HasCreditCardPayment)
            {
                var onlineEftMachineId = SystemService.GetSystemValue("OnlineEFTMachineId");
                eftMachine = AccountingService.GetEftMachines().Data.First(x => x.Id == Convert.ToInt32(onlineEftMachineId));
            }
            var paymentCreateRequestModel = new InvoicePaymentCreateModel
            {
                InvoiceNumber = null,
                PaymentType = HasCreditCardPayment?PaymentTypeDto.Eft: PaymentTypeDto.PayPal,
                Amount = RefundRequest.RefundAmount.GetValueOrDefault(),
                Date = DateTime.Now,
                Reference = HasCreditCardPayment ? RefundRequest.RefundTransactionId: $"{PAYPALTRANSPREFIX}-{RefundRequest.RefundTransactionId}" ,
                AccountId = paymentAccount,
                EftMachine = HasCreditCardPayment ? eftMachine.TerminalNumber:null,
                OrderNumber = GetOrderNumber()
            };

            return paymentCreateRequestModel;
        }

        protected abstract string GetOrderNumber();


        protected override IList<ActionEventLevel> GetActionEvents()
        {
            var events = base.GetActionEvents();

            if (HasCreditCardPayment)
            {
                var maxLevel = events.Max(x => x.Level);
                var creditCardEvent = new ActionEventLevel { Event = SystemActionEventTypeName.CreditCardRefundIssuedToApplicant, Level = maxLevel + 1 };
                events.Add(creditCardEvent);
            }
            if (HasPayPalPayment)
            {
                //TODO check if this outputs the correct email
                var maxLevel = events.Max(x => x.Level);
                var creditCardEvent = new ActionEventLevel { Event = SystemActionEventTypeName.CreditCardRefundIssuedToApplicant, Level = maxLevel + 1 };
                events.Add(creditCardEvent);
            }

            return events;
        }

        protected internal override ApplicationRefundCreateRequestModel GetRefundPreview()
        {
            if (RefundRequest != null)
            {
                var request = GetApplicationRefundCreateRequest(RefundRequest);

                if (request != null)
                {
                    var lineItem = GetCreditNoteLineItems();
                    request.LineItems = request.LineItems.Append(lineItem);
                    if (HasCreditCardPayment || HasPayPalPayment)
                    {
                        var payment = GetPayment();
                        request.Payments = request.Payments.Concat(new[] { payment }).ToArray();
                    }
                }
                return request;
            }
            return null;
        }

        private InvoiceLineItemModel GetCreditNoteLineItems()
        {
            var isSponsored = ApplicationModel.ApplicationInfo.SponsorInstitutionId != null;

            var workflowFee = CredentialRequestModel.CredentialWorkflowFees.Single(
                  workflowFeeItem => workflowFeeItem.Id == RefundRequest.CredentialWorkflowFeeId);

            var result = new InvoiceLineItemModel
            {
                EntityId = ApplicationModel.ApplicantDetails.EntityId,
                ProductSpecificationId = workflowFee.ProductSpecification.Id,
                Quantity = 1,
                Description = GetRefundItemDescription(workflowFee.ProductSpecification, isSponsored),
                IncGstCostPerUnit = RefundRequest.RefundAmount.GetValueOrDefault(),
                GstApplies = RefundRequest.InitialPaidTax != null && RefundRequest.InitialPaidTax > 0,
                ProductCode = workflowFee.ProductSpecification.Code,
                GlCode = workflowFee.ProductSpecification.GlCode,
            };

            return result; 
        }

        protected override string GetRefundItemDescription(ProductSpecificationModel feeProductSpec, bool isSponsored)
        {
            var workflowFee = CredentialRequestModel.CredentialWorkflowFees.Single(
                workflowFeeItem => workflowFeeItem.Id == RefundRequest.CredentialWorkflowFeeId);

            var description = base.GetRefundItemDescription(feeProductSpec, isSponsored);

            return $"{description} \n {workflowFee.InvoiceNumber}";
        }

        protected override bool CanProcessInBatch()
        {
            return true;
        }

        protected virtual CredentialRequestStatusTypeName GetCredentialRequestExitState()
        {
            if (ProcessInBatch)
            {
                return ProcessingCreditNoteStatus;
            }

            return CreditNotePaidStatus;
        }

        protected override void ProcessRefundRequest()
        {
            base.ProcessRefundRequest();
            if (HasCreditCardPayment || HasPayPalPayment)
            {
                RefundRequest.CreditNotePaymentProcessedDate = DateTime.Now;
            }
            else
            {
                RefundRequest.CreditNoteProcessedDate = DateTime.Now;
            }
        }
    }
}
