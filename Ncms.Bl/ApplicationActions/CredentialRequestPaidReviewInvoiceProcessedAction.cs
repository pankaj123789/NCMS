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
    public class CredentialRequestPaidReviewInvoiceProcessedAction : CredentialRequestCreatePaidTestReviewAction
    {
        protected override ProductSpecificationModel ActionFee => IsPaidInvoice ? null:  base.ActionFee;
        protected override SystemActionTypeName? OnInvoicePaymentActionType => null;
        protected override SystemActionTypeName? OnInvoiceCreationActionType => null;
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { ProcessingInvoiceStatus };
       
        protected override bool HasCreditCardPayment => IsPaidInvoice;

        protected bool IsPaidInvoice => WorkflowFee?.OnPaymentActionType != null && WorkflowFee.OnPaymentActionType.Value == InvoiceProcessedAction;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.FinanceOther;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Manage;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateMinimumCredentialRequests,
            ValidateMandatoryFields,
            ValidateMandatoryPersonFields,
            ValidateMandatoryDocuments,
            ValidateProcessedFee
        };


        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            SetEnteredUser,
            SetReceivingOffice,
            SeInvoiceDetails,
            UpdateCredentialRequestStatus,
            SetExitState,
            CreateInvoiceAttachment,
            ProcessFee
        };

        protected override bool CanProcessInBatch()
        {
            return false;
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

        protected override CredentialWorkflowFeeModel GetWorkflowFee()
        {
            var model = CredentialRequestModel.CredentialWorkflowFees
                .Where(x =>
                (x.OnPaymentActionType == InvoiceProcessedAction
                 && !string.IsNullOrWhiteSpace(x.PaymentReference) && x.InvoiceId.HasValue
                 && !x.PaymentActionProcessedDate.HasValue) ||
                (x.OnInvoiceActionType == InvoiceProcessedAction
                 && x.InvoiceId.HasValue
                 && !x.InvoiceActionProcessedDate.HasValue)).OrderByDescending(x => x.Id).FirstOrDefault();

            return model;
        }

        protected override void ProcessFee()
        {
            var fee = WorkflowFee;

            Output.PendingProcessFee = new ProcessFeeModel
            {
                CredentialWorkflowFeeId = fee.Id,
                Type = IsPaidInvoice ? ProcessTypeName.Payment : ProcessTypeName.Invoice
            };
            if (Output.PendingProcessFee.Type == ProcessTypeName.Payment)
            {
                Output.PendingProcessFee.PaymentReference = WizardModel.PaymentReference;
                Output.PendingProcessFee.TransactionId = WizardModel.TransactionId;
                Output.PendingProcessFee.OrderNumber = WizardModel.OrderNumber;
            }
        }

        protected override void CreateEmailAttachmentsIfApplicable()
        {
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
