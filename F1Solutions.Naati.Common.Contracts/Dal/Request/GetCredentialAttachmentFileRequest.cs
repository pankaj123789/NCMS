namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCredentialAttachmentFileRequest
    {
        public int StoredFileId { get; set; }
        public string TempFileStorePath { get; set; }
    }
}