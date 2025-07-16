using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class EmailMessageResultDto
    {
        public int EmailMessageId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserName { get; set; }
        public string RecipientEmail { get; set; }
        public DateTime? LastSendAttemptDate { get; set; }
        public string LastSendResult { get; set; }
        public string Subject { get; set; }
        public int EmailSendStatusTypeId { get; set; }
        public string EmailSendStatus { get; set; }
    }
}