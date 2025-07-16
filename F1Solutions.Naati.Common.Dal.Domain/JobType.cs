namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class JobType : LegacyEntityBase, ILookupType
    {
        public JobType(int id) : base(id)
        {
        }

        public JobType()
        {
        }

        public virtual string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
