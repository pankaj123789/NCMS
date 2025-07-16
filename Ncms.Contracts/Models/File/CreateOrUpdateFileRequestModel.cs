namespace Ncms.Contracts.Models.File
{
    public class CreateOrUpdateFileRequestModel
    {
        public string Type { get; set; }
        public string FilePath { get; set; }
        public string StoragePath { get; set; }
        public int UploadedByUserId { get; set; }
        public string Title { get; set; }
        public int StoredFileId { get; set; }
    }
}
