namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ContactPerson : EntityBase
    {
        public virtual Institution Institution { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string PostalAddress { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Inactive { get; set; }

        public override IAuditObject RootAuditObject => Institution;

        public override string ToString()
        {
            return Name;
        }
    }
}
