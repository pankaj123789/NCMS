namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class DocumentTypeRole : EntityBase
    {
        public virtual DocumentType DocumentType { get; set; }
        public virtual SecurityRole Role { get; set; }
        public virtual bool Upload { get; set; }
        public virtual bool Download { get; set; }
    }
}
