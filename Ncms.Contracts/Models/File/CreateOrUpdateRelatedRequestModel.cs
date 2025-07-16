namespace Ncms.Contracts.Models.File
{
    public class CreateOrUpdateRelatedRequestModel
    {
        public int Id { get; set; }
        public string File { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int StoredFileId { get; set; }
        public int RelatedId { get; set; }
        public bool TestAsset { get; set; }
        public bool EportalDownload { get; set; }
    }
}
