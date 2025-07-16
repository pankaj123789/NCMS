using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class EmailMessageDto
    {
        public string GraphEmailMessageId { get; set; }
        public int EmailMessageId { get; set; }
        public int RecipientEntityId { get; set; }
        public int RecipientUserId { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int CreatedUserId { get; set; }
        public string Bcc { get; set; }
        public string From { get; set; }
        public string Cc { get; set; }
        public DateTime? LastSendAttemptDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastSendResult { get; set; }
        public EmailMessageAttachmentDto[] Attachments { get; set; }
        public bool HasAttachments { get; set; }
        public string CreatedUserName { get; set; }
        public int EmailSendStatusTypeId { get; set; }
        public int? EmailTemplateId { get; set; }
    }
}