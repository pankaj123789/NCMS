namespace Ncms.Contracts.Models.File
{
    public class PostRequestModel
    {
        public int? RelatedId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public int StoredFileId { get; set; }
    }
}
