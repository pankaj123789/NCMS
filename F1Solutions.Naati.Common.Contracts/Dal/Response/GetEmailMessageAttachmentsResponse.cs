using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetEmailMessageAttachmentsResponse
    {        
        public EmailMessageAttachmentDto[] Attachments { get; set; }
    }
}