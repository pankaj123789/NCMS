using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class EmailMassageHeaderDto
    {
        public string GraphEmailMessageId { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTime? LastSendAttemptDate { get; set; }
        public bool HasAttachments { get; set; }
        public string MailBox { get; set; }
    }
}