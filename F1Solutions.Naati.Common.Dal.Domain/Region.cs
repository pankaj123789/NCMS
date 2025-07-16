namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Region : LegacyEntityBase, ILookupType
    {
        public Region(int id)
            : base(id)
        {
        }

        public Region()
        {
        }

        public virtual string Name { get; set; }
        public virtual State State { get; set; }
        public virtual Country Country { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
