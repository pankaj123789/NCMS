namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class NoteAttachment : EntityBase
    {
        public virtual Note Note { get; set; }
        public virtual StoredFile StoredFile { get; set; }
        public virtual string Description { get; set; }
    }
}