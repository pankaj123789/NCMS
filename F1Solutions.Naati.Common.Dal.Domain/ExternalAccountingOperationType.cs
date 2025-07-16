namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ExternalAccountingOperationType : EntityBase, ILookupType
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}