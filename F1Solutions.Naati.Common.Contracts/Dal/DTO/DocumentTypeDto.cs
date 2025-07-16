namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class DocumentTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool ExaminerToolsDownload { get; set; }
        public bool ExaminerToolsUpload { get; set; }
        public bool MergeDocument { get; set; }
    }
}
