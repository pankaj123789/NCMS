namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PayrollStatus : EntityBase, ILookupType
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
    }
}