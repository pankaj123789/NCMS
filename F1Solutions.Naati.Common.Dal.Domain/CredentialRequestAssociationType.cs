namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialRequestAssociationType : EntityBase, IDynamicLookupType
    {
        public virtual string DisplayName { get; set; }
        public virtual string Name { get; set; }
    }
}
