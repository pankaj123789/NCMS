namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class AssetType : LegacyEntityBase, ILookupType
    {
        public AssetType(int id)
            : base(id) { }
        public AssetType() { }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}
