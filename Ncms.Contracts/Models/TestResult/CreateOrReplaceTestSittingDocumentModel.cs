namespace Ncms.Contracts.Models.TestResult
{
    public class CreateOrReplaceTestSittingDocumentModel
    {
        public int? Id { get; set; }
        public int UploadedByUserId { get; set; }
        public string File { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int StoredFileId { get; set; }
        public int TestSittingId { get; set; }
        public bool EportalDownload { get; set; }
        public string FilePath { get; set; }
    }
}
