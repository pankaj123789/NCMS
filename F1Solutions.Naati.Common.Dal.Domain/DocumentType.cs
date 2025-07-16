namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class DocumentType : EntityBase, ILookupType, IDynamicLookupType
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool ExaminerToolsDownload { get; set; }
        public virtual bool ExaminerToolsUpload { get; set; }
        public virtual bool MergeDocument { get; set; }

        public virtual DocumentTypeCategory DocumentTypeCategory { get; set; }

      
    }
}
