using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models;

namespace Ncms.Contracts
{
    public class CredentialApplicationEmailMessageModel : EmailMessageModel
    {
        public int CredentialApplicationId { get; set; }
    }

    public class MaterialRequestEmailMessageModel : EmailMessageModel
    {
        public int MaterialRequestId { get; set; }
    }

    public class EmailQueueEmailMessageModel : EmailMessageModel
    {
        public string CreatedUserDisplayName { get; set; }
        public string RecipientDisplayName { get; set; }
    }

    public class EmailMassageHeaderModel
    {
        public string GraphEmailMessageId { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTime? LastSendAttemptDate { get; set; }
        public bool HasAttachments { get; set; }
        public string MailBox { get; set; }
    }

    public class EmailMessageModel
    {
        public int EmailMessageId { get; set; }
        public string RecipientEmail { get; set; }
        public DateTime? LastSendAttemptDate { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string LastSendResult { get; set; }
        public string Bcc { get; set; }
        public string Cc { get; set; }
        public string From { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public int RecipientUserId { get; set; }
        public IList<EmailMessageAttachmentModel> Attachments { get; set; }
        public string EmailContent => Body;
        public string To => RecipientEmail;
        public string GraphEmailMessageId { get; set; }
        public bool HasAttachments { get; set; }
        public int RecipientEntityId { get; set; }
        public int EmailSendStatusTypeId { get; set; }
        public string EmailSendStatus { get; set; }
        public int? EmailTemplateId { get; set; }
    }

    public interface IEmailMessageService
    {
        GenericResponse<EmailMessageModel> GetGenericEmailMessage(int id);
        GenericResponse<IEnumerable<EmailQueueEmailMessageModel>> GetEmailMessageQueue(SearchRequest request);
        GenericResponse<CredentialApplicationEmailMessageModel> CreateEmailMessage(CredentialApplicationEmailMessageModel model);
        GenericResponse<MaterialRequestEmailMessageModel> CreateEmailMessage(MaterialRequestEmailMessageModel model);

        GenericResponse<EmailMessageModel> CreateGenericEmailMessage(EmailMessageModel model);

        BusinessServiceResponse SendEmailMessage(int emailMessageId);
        BusinessServiceResponse SendEmailMessageById(int emailMessageId);
        GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>> GetEmailMessageByCredentialApplicationId(int creadentialApplicationId);
        GenericResponse<IEnumerable<EmailMessageAttachmentModel>> ListAttachments(int emailMessageId);
        GenericResponse<IEnumerable<EmailMassageHeaderModel>> GetMailMessagesFromSharedAccount(List<string> sharedAccount, int naatiNumber);
        GenericResponse<EmailMessageModel> GetEmailDetails(string graphEmailId, string userEmail);
        GenericResponse<IEnumerable<EmailMessageAttachmentModel>> GetMailAttachment(string graphEmailId, string userEmail);
        GetEmailTemplateResponse GetSystemEmailTemplate(GetSystemEmailTemplateRequest request);

        GenericResponse<IEnumerable<MaterialRequestEmailMessageModel>> GetEmailMessageByMaterialRequestId(int materialRequestId);
    }

    public enum AttachmentType
    {
        Invoice = 1,
        CredentialDocument = 2,
        CandidateBrief = 3,
        RoundRequestDocuments = 4,
        RoundResponseDocuments = 5,
        CreditNote = 6
    }

    public class EmailMessageAttachmentModel
    {
        public AttachmentType AttachmentType { get; set; }
        public int EmailMessageAttachmentId { get; set; }
        public int EmailMessageId { get; set; }
        public int StoredFileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string StoragePath { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public string UploadedByName { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string Type { get; set; }
        public int? GraphAttachmentSize { get; set; }
        public byte[] GraphAttachmentBytes { get; set; }
        public bool IsInline { get; set; }
        public string ContentId { get; set; }
    }
}

