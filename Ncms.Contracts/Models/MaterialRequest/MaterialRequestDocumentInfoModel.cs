namespace Ncms.Contracts.Models.MaterialRequest
{
    public abstract class BaseMaterialRequestDocumentModel
    {
        public int AttachmentId { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public int DocumentTypeId { get; set; }
     

        public int StoredFileId { get; set; }
        public int UserId { get; set; }
        public  bool ExaminersAvailable { get; set; }
    }

    public class OutputTestMaterialDocumentInfoModel : BaseMaterialRequestDocumentModel
    {
        public bool MergeDocument { get; set; }
    }

    public class MaterialRequestDocumentInfoModel : BaseMaterialRequestDocumentModel
    {
        public bool NcmsAvailable { get; set; }
    }

}
