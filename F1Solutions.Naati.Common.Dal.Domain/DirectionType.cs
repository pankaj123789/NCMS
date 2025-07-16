namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class DirectionType : EntityBase, IDynamicLookupType
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public override string ToString()
        {
            return this.DisplayName;
        }
    }

	public enum DirectionTypeName
	{
		L1ToL2 = 1,
		L2ToL1 = 2,
		L1AndL2 = 3,
		L1 = 4
	}
}
