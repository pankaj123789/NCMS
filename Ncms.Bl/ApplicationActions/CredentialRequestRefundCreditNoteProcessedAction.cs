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
    public class CredentialRequestRefundCreditNoteProcessedAction : CredentialRequestProcessRefundAction
    {
        protected override SystemActionTypeName? OnCreditNotePaymentActionType => null;
        protected override SystemActionTypeName? OnCreditNoteCreationActionType => null;
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { ProcessingCreditNoteStatus };
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.ProcessRefund;
        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateRefundFields,
        };
        
        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            SetCreditNoteDetails,
            SetApplicationStatus,
            SetExitState,
            //removing with Wiise integration as the emails are sent by wiise
            //CreateCreditNoteAttachment,
            ProcessRefundRequest,
        };

        protected override void SetApplicationStatus()
        {
            //if (HasCreditCardPayment || HasPayPalPayment) Direct deposits invoices are posted at the creation (Wiise integration)172377
            //{
            base.SetApplicationStatus();
            //}
        }

        protected override bool CanProcessInBatch()
        {
            return false;
        }

        protected override void ValidateRefundFields()
        {
            base.ValidateRefundFields();
            if ((HasCreditCardPayment || HasPayPalPayment) && string.IsNullOrWhiteSpace(RefundRequest.PaymentReference))
            {
                throw new UserFriendlySamException(Naati.Resources.Application.PaymentReferenceNotFoundErrorMessage);
            }

            if(!(HasCreditCardPayment || HasPayPalPayment) && RefundRequest.CreditNoteId == null)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.CreditNotFoundErrorMessage);
            }
        }

        protected virtual void SetCreditNoteDetails()
        {
            if (!RefundRequest.CreditNoteId.HasValue || string.IsNullOrWhiteSpace(RefundRequest.CreditNoteNumber))
            {
                throw new Exception("Credit Note  details must be set");
            }

            Output.CreditNoteId = RefundRequest.CreditNoteId.GetValueOrDefault();
            Output.CreditNoteNumber = RefundRequest.CreditNoteNumber;
        }

        protected override void CreateEmailAttachmentsIfApplicable()
        {
        }
        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            if ((HasCreditCardPayment || HasPayPalPayment))
            {
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.PaymentReference), RefundRequest.PaymentReference);
            }
        }
      
        protected virtual void CreateCreditNoteAttachment()
        {
            if (!string.IsNullOrEmpty(Output.CreditNoteNumber))
            {
                var creditNoteTempFilePath = GetCreditNoteToTempFile(Output.CreditNoteId, Output.CreditNoteNumber);

                var type = StoredFileType.CreditNote.ToString();
                var fileSaveRequest = new CreateOrUpdateFileRequestModel
                {
                    Title = $"CreditNote {Output.CreditNoteNumber}",
                    Type = type,
                    FilePath = creditNoteTempFilePath,
                    UploadedByUserId = CurrentUser.Id,
                    StoragePath = $@"{type}\{Path.GetFileName(creditNoteTempFilePath)}",
                };
                var storedFile = FileService.Create(fileSaveRequest);

                Attachments.Add(new EmailMessageAttachmentModel
                {
                    StoredFileId = storedFile.StoredFileId,
                    AttachmentType = AttachmentType.CreditNote
                });
            }
        }

        protected override void ProcessRefundRequest()
        {
            if (HasCreditCardPayment || HasPayPalPayment)
            {
                RefundRequest.CreditNotePaymentProcessedDate = DateTime.Now;
            }
            else
            {
                RefundRequest.CreditNoteProcessedDate = DateTime.Now;
            }
            RefundRequest.ObjectStatusId = (int)ObjectStatusTypeName.Updated;
        }
    }
}
