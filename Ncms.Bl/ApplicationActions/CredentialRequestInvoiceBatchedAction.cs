using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public abstract class CredentialRequestInvoiceBatchedAction : CredentialRequestStateAction
    {
        protected virtual bool HasCreditCardPayment => !string.IsNullOrEmpty(WizardModel.PaymentReference) && (!WizardModel.PaymentReference.StartsWith("PAYPAL")) &&
                                                       WizardModel.PaymentAmount.HasValue;
        protected virtual bool HasPayPalPayment => !string.IsNullOrEmpty(WizardModel.PaymentReference) && (WizardModel.PaymentReference.StartsWith("PAYPAL")) &&
                                                       WizardModel.PaymentAmount.HasValue;
        protected abstract SystemActionTypeName InvoiceProcessedAction { get; }
        protected abstract SystemActionTypeName InovicePaidAction { get; }
        protected abstract CredentialRequestStatusTypeName ProcessingInvoiceStatus { get; }
        protected abstract CredentialRequestStatusTypeName PendingPaymentStatus { get; }
        protected abstract CredentialRequestStatusTypeName InovicePaidStatus { get; }
        protected override CredentialRequestStatusTypeName CredentialRequestExitState => GetCredentialrequestExitState();
        protected override SystemActionTypeName? OnInvoiceCreationActionType => GetOnInvoiceCreationActionType();
        protected override SystemActionTypeName? OnInvoicePaymentActionType => GetOnInvoicePaymentActionType();

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            if (HasCreditCardPayment || HasPayPalPayment)
            {
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PaymentReference), WizardModel.PaymentReference);
            }

        }

        protected virtual SystemActionTypeName? GetOnInvoiceCreationActionType()
        {
            if (ProcessInBatch && !(HasCreditCardPayment || HasPayPalPayment))
            {
                return InvoiceProcessedAction;
            }
            return null;
        }

        protected virtual SystemActionTypeName? GetOnInvoicePaymentActionType()
        {
            if ((ProcessInBatch && (HasCreditCardPayment || HasPayPalPayment)))
            {
                return InvoiceProcessedAction;
            }

            return null;
        }

        protected virtual InvoicePaymentCreateModel GetCreditCardPayment()
        {
            if (!(HasCreditCardPayment))
            {
                throw new Exception("Payment details need to be specified.");
            }

            var paymentAccount = new Guid(SystemService.GetSystemValue("WiisePaymentAccount"));
            var onlineEftMachineId = SystemService.GetSystemValue("OnlineEFTMachineId");

            var eftMachine = AccountingService.GetEftMachines().Data.First(x => x.Id == Convert.ToInt32(onlineEftMachineId));

            var paymentCreateRequestModel = new InvoicePaymentCreateModel
            {
                InvoiceNumber = null,
                PaymentType = PaymentTypeDto.Eft,
                Amount = WizardModel.PaymentAmount.GetValueOrDefault(),
                Date = DateTime.Now,
                Reference = WizardModel.TransactionId,
                AccountId = paymentAccount,
                EftMachine = eftMachine.TerminalNumber,
                OrderNumber = WizardModel.OrderNumber
            };

            return paymentCreateRequestModel;
        }

        protected virtual InvoicePaymentCreateModel GetPayPalPayment()
        {
            if (!(HasPayPalPayment))
            {
                throw new Exception("Payment details need to be specified.");
            }

            var paymentAccount = new Guid(SystemService.GetSystemValue("WiisePaymentAccount"));


            var paymentCreateRequestModel = new InvoicePaymentCreateModel
            {
                InvoiceNumber = null,
                PaymentType = PaymentTypeDto.PayPal,
                Amount = WizardModel.PaymentAmount.GetValueOrDefault(),
                Date = DateTime.Now,
                Reference = WizardModel.PaymentReference,
                AccountId = paymentAccount,
                //EftMachine = eftMachine.TerminalNumber,
                OrderNumber = WizardModel.OrderNumber
            };

            return paymentCreateRequestModel;
        }

        protected override IList<ActionEventLevel> GetActionEvents()
        {
            var events = base.GetActionEvents();

            if (HasCreditCardPayment)
            {
                var maxLevel = events.Max(x => x.Level);
                var creditCardEvent = new ActionEventLevel { Event = SystemActionEventTypeName.CreditCardPaymentReceived, Level = maxLevel + 1 };
                events.Add(creditCardEvent);
            }
            if (HasPayPalPayment)
            {
                //TODO fix this for PayPal. Seems linked to emails
                var maxLevel = events.Max(x => x.Level);
                var payPalEvent = new ActionEventLevel { Event = SystemActionEventTypeName.CreditCardPaymentReceived, Level = maxLevel + 1 };
                events.Add(payPalEvent);
            }

            return events;
        }

        protected internal override ApplicationInvoiceCreateRequestModel GetInvoicePreview()
        {
            var feeProductSpec = ActionFee;

            if (feeProductSpec != null)
            {
                var request = GetApplicationInvoiceCreateRequest(feeProductSpec);

                if (request != null)
                {
                    if (HasCreditCardPayment)
                    {
                        var payment = GetCreditCardPayment();
                        request.Payments = request.Payments.Concat(new[] { payment }).ToArray();
                        return request;
                    }
                    if (HasPayPalPayment)
                    {
                        var payment = GetPayPalPayment();
                        request.Payments = request.Payments.Concat(new[] { payment }).ToArray();
                        //here, we are oprocessing invoices and payment at the same time and 
                        //it has come form paypal. A surcharge should apply. Calualte and add as an invoice line item
                        if(payment.Amount > feeProductSpec.CostPerUnit)
                        {
                            var newLineItems = request.LineItems.ToList();
                            var amount = payment.Amount - feeProductSpec.CostPerUnit;
                            newLineItems.Add(new InvoiceLineItemModel(){
                                EntityId = ApplicationModel.ApplicantDetails.EntityId,
                                ProductSpecificationId = 0,
                                Quantity = 1,
                                Description = "PayPal Loading",
                                IncGstCostPerUnit = amount,
                                GstApplies = false,
                                ProductCode = null,
                                GlCode = SystemService.GetSystemValue("PayPalGlCode")
                            });
                            request.LineItems = newLineItems;
                        }
                        return request;
                    }
                    if(IsSponsored)
                    {
                        return request;
                    }
                    //no payment exists. Cannot continue to create invoice
                    return null;
                }
            }
            return null;
        }

        protected override bool CanProcessInBatch()
        {
            return InvoiceRequired;
        }

        protected virtual CredentialRequestStatusTypeName GetCredentialrequestExitState()
        {
            if (ProcessInBatch)
            {
                return ProcessingInvoiceStatus;
            }

            return (InvoiceRequired && !IsSponsored)? PendingPaymentStatus : InovicePaidStatus;
        }
    }
}
