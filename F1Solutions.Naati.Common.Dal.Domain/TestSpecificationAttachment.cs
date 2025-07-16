namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSpecificationAttachment : EntityBase
    {
        public virtual StoredFile StoredFile { get; set; }
        public virtual TestSpecification TestSpecification { get; set; }
        public virtual string Title { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual bool MergeDocument { get; set; }
        public virtual bool ExaminerToolsDownload { get; set; }
    }
}
