namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class WorkPracticeAttachment : EntityBase
    {
        public virtual StoredFile StoredFile { get; set; }
        public virtual string Description { get; set; }
        public virtual WorkPractice WorkPractice { get; set; }
    }
}
