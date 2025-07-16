namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestPanelMembershipType : EntityBase, IDynamicLookupType
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
    }
}
