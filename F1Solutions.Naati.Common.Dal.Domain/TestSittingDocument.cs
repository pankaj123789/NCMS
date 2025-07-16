namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSittingDocument : EntityBase
    {
        public virtual TestSitting TestSitting { get; set; }
        public virtual StoredFile StoredFile { get; set; }
        public virtual string Title { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual bool EportalDownload { get; set; }
    }
}