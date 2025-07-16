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
    public class ApplicationSubmitAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.Draft };
        protected override ProductSpecificationModel ActionFee => GetFee(FeeTypeName.Application);

        protected override CredentialApplicationStatusTypeName ApplicationExitState => GetApplicationExitState();

        protected virtual bool HasCreditCardPayment => !string.IsNullOrEmpty(WizardModel.PaymentReference) &&
                                                       WizardModel.PaymentAmount.HasValue;


        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateMinimumCredentialRequests,
                                                              ValidateMandatoryFields,
                                                              ValidateMandatoryPersonFields,
                                                              ValidateMandatoryDocuments,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetEnteredUser,
                                                              SetReceivingOffice,
                                                              CreateInvoiceIfApplicable,
                                                              UpdateCredentialRequestStatus,
                                                              AttachPdActivities,
                                                              AttachWorkPractices,
                                                              SetExitState
                                                          };
     
        protected virtual CredentialApplicationStatusTypeName GetApplicationExitState()
        {
            if (ProcessInBatch)
            {
                return CredentialApplicationStatusTypeName.ProcessingSubmission;
            }

            return (InvoiceRequired && !IsSponsored) ? CredentialApplicationStatusTypeName.AwaitingApplicationPayment :
                    ApplicationType.RequiresChecking ? CredentialApplicationStatusTypeName.Entered : CredentialApplicationStatusTypeName.InProgress;
        }

        private SystemActionTypeName? GetOnInvoiceCreationActionType()
        {

            if (ProcessInBatch && !HasCreditCardPayment)
            {
                return SystemActionTypeName.ApplicationSubmissionProcessed;
            }
            return null;
        }


        private SystemActionTypeName? GetOnInvoicePaymentActionType()
        {
            if ((ProcessInBatch && HasCreditCardPayment))
            {
                return SystemActionTypeName.ApplicationSubmissionProcessed;
            }

            return InvoiceRequired && !IsSponsored
                ? SystemActionTypeName.ApplicationInvoicePaid
                : (SystemActionTypeName?)null;
        }

        protected override SystemActionTypeName? OnInvoiceCreationActionType =>
            GetOnInvoiceCreationActionType();

        protected override SystemActionTypeName? OnInvoicePaymentActionType =>
            GetOnInvoicePaymentActionType();


        protected override bool CanProcessInBatch()
        {
            return InvoiceRequired;
        }

        protected virtual void AttachPdActivities()
        {
            if (ApplicationModel.ApplicationType.Category != CredentialApplicationTypeCategoryName.Recertification)
            {
                return;
            }
            var certificationPeriodId = ApplicationModel.ApplicationInfo.CertificationPeriodId;
            if (certificationPeriodId == 0)
            {
                throw new Exception("Certification Period not found");
            }

            var activityIds = ActivityPointsService.CaluculatePointsFor(ApplicationModel.ApplicationInfo.NaatiNumber, certificationPeriodId).IncludedActivitiesIds;

            foreach (var activityId in activityIds)
            {
                ApplicationModel.PdActivities.Add(new PdActivityFieldModel
                {
                    ActivityId = activityId,
                    ObjectStatusId = (int)ObjectStatusTypeName.Created
                });
            }
        }

        protected override void UpdateCredentialRequestStatus()
        {
            // UC5000 BR6
            foreach (var credentialRequest in ApplicationModel.CredentialRequests)
            {
                int statusId = credentialRequest.StatusTypeId;

                if (InvoiceRequired && ApplicationType.RequiresChecking)
                {
                    statusId = (int)CredentialRequestStatusTypeName.AwaitingApplicationPayment;
                }
                else if (ApplicationType.RequiresChecking)
                {
                    statusId = (int)CredentialRequestStatusTypeName.RequestEntered;
                }
                else
                {
                    var credentialApplicationTypeCredentialType = credentialRequest.CredentialType.CredentialApplicationTypeCredentialTypes.FirstOrDefault(c => c.CredentialApplicationTypeId == ApplicationModel.ApplicationType.Id);
                    if (ApplicationType.RequiresAssessment)
                    {
                        statusId = (int)CredentialRequestStatusTypeName.ReadyForAssessment;
                    }
                    else if (credentialApplicationTypeCredentialType != null && credentialApplicationTypeCredentialType.HasTest && !credentialApplicationTypeCredentialType.HasTestFee)
                    {
                        statusId = (int)CredentialRequestStatusTypeName.TestAccepted;
                    }
                    else
                    {
                        statusId = (int)CredentialRequestStatusTypeName.EligibleForTesting;
                    }
                }

                if (ProcessInBatch)
                {
                    statusId = (int)CredentialRequestStatusTypeName.ProcessingSubmission;
                }

                if (statusId != credentialRequest.StatusTypeId)
                {
                    credentialRequest.StatusTypeId = statusId;
                    credentialRequest.StatusChangeUserId = CurrentUser.Id;
                    credentialRequest.StatusChangeDate = DateTime.Now;
                }
            }
        }

        protected internal override ApplicationInvoiceCreateRequestModel GetInvoicePreview()
        {
            var feeProductSpec = ActionFee;
            if (feeProductSpec != null)
            {
                var model = GetApplicationInvoiceCreateRequest(feeProductSpec);

                if (HasCreditCardPayment)
                {
                    var payment = GetPayment();
                    model.Payments = model.Payments.Concat(new[] { payment }).ToArray();
                }

                return model;


            }
            return null;
        }

        protected virtual InvoicePaymentCreateModel GetPayment()
        {
            if (!HasCreditCardPayment)
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

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            if (HasCreditCardPayment)
            {
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PaymentReference), WizardModel.PaymentReference);
            }

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

            return events;
        }

    }
}