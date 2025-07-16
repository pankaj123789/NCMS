using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class WorkPracticeAttachmentResponse
    {
        public int Id { get; set; }
        public StoredFileDto StoredFile { get; set; }
        public string Description { get; set; }
    }
}