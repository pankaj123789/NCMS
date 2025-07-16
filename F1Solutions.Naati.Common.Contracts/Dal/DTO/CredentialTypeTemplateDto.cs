using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialTypeTemplateDto
    {
        public StoredFileType TemplateDocumentType { get; set; }
        public StoredFileType OutputDocumentType { get; set; }
        public string DocumentNameTemplate { get; set; }
        public int NextDocumentIdentifier{ get; set; }
        public string FilePath { get; set; }
    }
}
