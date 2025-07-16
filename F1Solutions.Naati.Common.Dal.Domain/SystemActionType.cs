namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SystemActionType : EntityBase, IDynamicLookupType
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public override string ToString()
        {
            return DisplayName;
        }
    }
}