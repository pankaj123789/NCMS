namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrUpdateFileRequest : FileMetadataRequest
    {
        public string FilePath { get; set; }        
        public string TokenToRemoveFromFilename { get; set; }
    }

    public class MoveFileRequest
    {
        public int StoredFileId { get; set; }
        public string StotoragePath { get; set; }
    }
}