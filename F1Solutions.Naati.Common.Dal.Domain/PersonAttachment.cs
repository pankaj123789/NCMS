namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PersonAttachment : EntityBase
    {
        public virtual Person Person { get; set; }

        public virtual StoredFile StoredFile { get; set; }

        public virtual string Description { get; set; }

        public override IAuditObject RootAuditObject => Person;
    }

}
