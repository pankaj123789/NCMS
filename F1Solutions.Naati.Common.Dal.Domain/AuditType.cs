namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class AuditType : LegacyEntityBase, ILookupType
    {
        public AuditType(int id) : base(id)
        {
        }

        public AuditType()
        {
        }

        public virtual string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
