using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class EmailMessageRequest <T> where T : EmailMessageDto
    {
        public T EmailMessage { get; set; }
        public EmailSendStatusTypeName FailureStatus { get; set; }
    }
}