namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public abstract class BaseMaterialRequestDocumentDto
    {
        public int AttachmentId { get; set; }
        public string FilePath { get; set; }
        public  string Description { get; set; }
        public  string FileName { get; set; }
        public  int DocumentTypeId { get; set; }
        public  int? UserId { get; set; }
        public  int? PersonId { get; set; }
        public int StoredFileId { get; set; }
        public  bool ExaminersAvailable { get; set; }
     
    }

    public class OutputTestMaterialDocumentInfoDto : BaseMaterialRequestDocumentDto
    {
        public bool MergeDocument { get; set; }

   

    }

    public class MaterialRequestDocumentInfoDto : BaseMaterialRequestDocumentDto
    {
        public bool NcmsAvailable { get; set; }
    }
}
