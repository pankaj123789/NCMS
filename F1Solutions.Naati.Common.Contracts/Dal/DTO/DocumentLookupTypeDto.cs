namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class DocumentLookupTypeDto : LookupTypeDto
    {
        public int DocumentTypeId { get; set; }

        public string UploadedBy { get; set; }

        public string FileType { get; set; }
        public long Size { get; set; }
        public bool ExaminersAvailable { get; set; }
        public bool MergeDocument { get; set; }
    }
}
