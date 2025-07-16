using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl.ApplicationActions
{

    public class MakeApplicationPaymentAction : ApplicationInvoicePaidAction
    {
        protected override ProductSpecificationModel ActionFee => GetFee(FeeTypeName.ApplicationAssessment);
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.ProcessingApplicationInvoice;
        protected virtual CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestAccepted;
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.AwaitingAssessmentPayment };


        protected virtual bool HasCreditCardPayment => !string.IsNullOrEmpty(WizardModel.PaymentReference) && (!WizardModel.PaymentReference.StartsWith("PAYPAL")) &&
                                                      WizardModel.PaymentAmount.HasValue;
        protected virtual bool HasPayPalPayment => !string.IsNullOrEmpty(WizardModel.PaymentReference) && (WizardModel.PaymentReference.StartsWith("PAYPAL")) &&
                                                       WizardModel.PaymentAmount.HasValue;
        
        protected override IList<Action> Preconditions => new List<Action>
                                                            {
                                                                ValidateEntryState,
                                                                ValidateUserPermissions ,
                                                                ValidateMandatoryFields,
                                                                ValidateMandatoryPersonFields,
                                                                ValidateMandatoryDocuments
                                                            };

        protected override IList<Action> SystemActions => new List<Action>
                                                            {
                                                                SetOwner,
                                                                CreateInvoiceAndPayment,
                                                                SetExitState,
                                                                //UpdateCredentialRequestStatus, 172406
                                                            };

        protected override void UpdateCredentialRequestStatus()
        {
            foreach (var credentialRequest in ApplicationModel.CredentialRequests)
            {
                credentialRequest.StatusTypeId = (int)CredentialRequestExitState;
                credentialRequest.StatusChangeUserId = CurrentUser.Id;
                credentialRequest.StatusChangeDate = DateTime.Now;
            }
        }

        protected void CreateInvoiceAndPayment()
        {
            var paymentAccount = new Guid(SystemService.GetSystemValue("WiisePaymentAccount"));
            var onlineEftMachineId = SystemService.GetSystemValue("OnlineEFTMachineId");
            var eftMachine = AccountingService.GetEftMachines().Data.First(x => x.Id == Convert.ToInt32(onlineEftMachineId));

            var feeProductSpec = ActionFee;
            if (feeProductSpec != null)
            {
                var request = GetApplicationInvoiceCreateRequest(feeProductSpec);

                request.PaymentCompletionAction = SystemActionTypeName.AssessmentInvoicePaid;
                request.InvoiceCompletionAction = null;
                request.Payments = new List<InvoicePaymentCreateModel>()
                {
                    new InvoicePaymentCreateModel()
                    {
                        AccountId = Guid.NewGuid(),
                        Amount= WizardModel.PaymentAmount.HasValue?WizardModel.PaymentAmount.Value:0,
                        Reference = WizardModel.PaymentReference,
                        Date = DateTime.Now,
                        PaymentType = HasCreditCardPayment?PaymentTypeDto.Eft: PaymentTypeDto.PayPal,
                        EftMachine = eftMachine.TerminalNumber,
                        OrderNumber = WizardModel.OrderNumber
                    }
                }.ToArray();

                if (HasPayPalPayment)
                {
                    var payment = request.Payments.First();
                    if (payment.Amount > feeProductSpec.CostPerUnit)
                    {
                        var newLineItems = request.LineItems.ToList();
                        var amount = payment.Amount - feeProductSpec.CostPerUnit;
                        newLineItems.Add(new InvoiceLineItemModel()
                        {
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
                }

                var response = AccountingService.CreateApplicationInvoice(request);
            }
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PaymentReference), WizardModel.PaymentReference);
        }



    }
}
