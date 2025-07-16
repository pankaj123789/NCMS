using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetMaterialRequestAttachmentsResponse
    {
        public MaterialRequestRoundAttachmentDto[] Attachments { get; set; }
    }
}
