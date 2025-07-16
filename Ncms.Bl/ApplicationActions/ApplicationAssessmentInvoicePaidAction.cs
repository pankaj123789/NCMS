using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.File;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationAssessmentInvoicePaidAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.ProcessingApplicationInvoice };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.InProgress;

        protected override IList<Action> Preconditions => new List<Action>
                                                                        {
                                                                            ValidateEntryState,
                                                                            ValidateUserPermissions,
                                                                            ValidateAllApplicationInvoices,
                                                                            ValidateProcessedFee
                                                                        };

        protected override IList<Action> SystemActions => new List<Action>
                                                                        {
                                                                            ClearOwner,
                                                                            CreateNote,
                                                                            SetExitState,
                                                                            SeInvoiceDetails,
                                                                            UpdateCredentialRequestStatus,
                                                                            CreateInvoiceAttachment,
                                                                            ProcessFee,
                                                                        };


        protected override void UpdateCredentialRequestStatus()
        {
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

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.FinanceOther;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Manage;


        protected override CredentialWorkflowFeeModel GetWorkflowFee()
        {
            var fees = ApplicationModel.CredentialWorkflowFees
                .Where(x => x.OnPaymentActionType == SystemActionTypeName.AssessmentInvoicePaid
                            && x.PaymentActionProcessedDate == null)
                .ToList();

            if (fees.Count > 1)
            {
                throw new UserFriendlySamException(
                    $"{fees.Count} assessment fees found for APP{ApplicationModel.ApplicationInfo.ApplicationId}. Expecting only 1.");
            }

            return fees.SingleOrDefault();
        }

        protected virtual void SeInvoiceDetails()
        {
            if (!WizardModel.InvoiceId.HasValue || string.IsNullOrWhiteSpace(WizardModel.InvoiceNumber))
            {
                throw new Exception("Invoice details must be set");
            }

            Output.InvoiceId = WizardModel.InvoiceId.GetValueOrDefault();
            Output.InvoiceNumber = WizardModel.InvoiceNumber;
        }

        protected virtual void CreateInvoiceAttachment()
        {
            if (!string.IsNullOrEmpty(Output.InvoiceNumber))
            {
                var invoiceTempFilePath = GetInvoiceToTempFile(Output.InvoiceId, Output.InvoiceNumber);

                var type = "Invoice";
                var fileSaveRequest = new CreateOrUpdateFileRequestModel
                {
                    Title = $"Invoice {Output.InvoiceNumber}",
                    Type = type,
                    FilePath = invoiceTempFilePath,
                    UploadedByUserId = CurrentUser.Id,
                    StoragePath = $@"{type}\{Path.GetFileName(invoiceTempFilePath)}",
                };
                var storedFile = FileService.Create(fileSaveRequest);

                Attachments.Add(new EmailMessageAttachmentModel
                {
                    StoredFileId = storedFile.StoredFileId,
                    AttachmentType = AttachmentType.Invoice
                });
            }
        }
    }
}
