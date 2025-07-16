namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ProfessionalDevelopmentActivityAttachmentDto
    {
        public StoredFileDto StoredFile { get; set; }
        public string Description { get; set; }
        public ProfessionalDevelopmentActivityDto ProfessionalDevelopmentActivity { get; set; }
        public int Id { get; set; }
    }
}