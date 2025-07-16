using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Accounting;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationFinishCheckingAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.BeingChecked };
        protected override ProductSpecificationModel ActionFee => GetFee(FeeTypeName.ApplicationAssessment);

        protected override CredentialApplicationStatusTypeName ApplicationExitState =>
            InvoiceRequired && !IsSponsored
                ? CredentialApplicationStatusTypeName.AwaitingAssessmentPayment
                : CredentialApplicationStatusTypeName.InProgress;

        protected override SystemActionTypeName? OnInvoicePaymentActionType =>
            IsSponsored
                ? (SystemActionTypeName?)null
                : SystemActionTypeName.AssessmentInvoicePaid;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateMinimumCredentialRequests,
                                                              ValidateMandatoryFields,
                                                              ValidateMandatoryPersonFields,
                                                              ValidateMandatoryDocuments,
                                                              ValidateNotTrustedApplicationInvoices,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              CreateInvoiceIfApplicable,
                                                              SetExitState,
                                                              UpdateCredentialRequestStatus,
                                                          };

        
        protected internal override ApplicationInvoiceCreateRequestModel GetInvoicePreview()
        {
            var feeProductSpec = ActionFee;
            if (feeProductSpec != null)
            {
                var model = GetApplicationInvoiceCreateRequest(feeProductSpec);
                return model;
            }
            return null;
        }

        protected override ApplicationInvoiceCreateRequestModel GetApplicationInvoiceCreateRequest(ProductSpecificationModel feeProductSpec)
        {
            var model = base.GetApplicationInvoiceCreateRequest(feeProductSpec);

            if (string.IsNullOrEmpty(model.Reference))
            {

                var office = ApplicationService
                   .GetLookupType(LookupType.OfficeAbbreviation.ToString()).Data.FirstOrDefault(x => x.Id == CurrentUser.OfficeId);

                model.Reference = office?.DisplayName + " - " + ApplicationModel.ApplicationInfo.ApplicationReference;
            }

            return model; 
        }

        protected override void SetExitState()
        {
            ApplicationModel.ApplicationInfo.ApplicationStatusTypeId = (int)ApplicationExitState;
            ApplicationModel.ApplicationInfo.StatusChangeUserId = CurrentUser.Id;
            ApplicationModel.ApplicationInfo.StatusChangeDate = DateTime.Now;
        }
        protected override void UpdateCredentialRequestStatus()
        {
            // UC 5000 BR1
            if (InvoiceRequired & !ApplicationModel.ApplicationInfo.SponsorInstitutionId.HasValue)
            {
                return;
            }

            foreach (var credentialRequest in ApplicationModel.CredentialRequests)
            {
                if (ApplicationType.RequiresAssessment)
                {
                    if (ApplicationType.Category == CredentialApplicationTypeCategoryName.Recertification && ApplicationModel.ApplicantDetails.AllowAutoRecertification)
                    {
                        credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.ProcessingRequest;
                    }
                    else
                    {
                        credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.ReadyForAssessment;
                    }
                }
                else
                {
                    var credentialApplicationTypeCredentialType = credentialRequest.CredentialType.CredentialApplicationTypeCredentialTypes.FirstOrDefault(c => c.CredentialApplicationTypeId == ApplicationModel.ApplicationType.Id);
                    if (credentialApplicationTypeCredentialType != null && credentialApplicationTypeCredentialType.HasTest && credentialApplicationTypeCredentialType.HasTestFee)
                    {
                        credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.EligibleForTesting;
                    }
                    else if (credentialApplicationTypeCredentialType != null && credentialApplicationTypeCredentialType.HasTest && !credentialApplicationTypeCredentialType.HasTestFee)
                    {
                        credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.TestAccepted;
                    }
                    else
                    {
                        credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.AssessmentComplete;
                    }
                }
                credentialRequest.StatusChangeUserId = CurrentUser.Id;
                credentialRequest.StatusChangeDate = DateTime.Now;
            }
        }

        protected override void CreatePendingEmailIfApplicable()
        {
            Output.PendingEmails = GetEmailPreviews();
        }

        /// <summary>
        /// any errors return the one message
        /// </summary>
        protected override void CreateInvoiceIfApplicable()
        {
            if (IsSponsored)
            {
                try
                {
                    base.CreateInvoiceIfApplicable();
                }
                catch(UserFriendlySamException)
                {
                    throw new UserFriendlySamException("An error has occurred. Please contact support.");
                }
            }
        }
    }
}