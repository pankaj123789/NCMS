using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetAttachmentsResponse
    {
        public NoteAttachmentDto[] Attachments { get; set; }
    }
}