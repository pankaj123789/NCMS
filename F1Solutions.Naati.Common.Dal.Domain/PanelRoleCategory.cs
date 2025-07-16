namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PanelRoleCategory : EntityBase, IDynamicLookupType
    {
        public virtual  string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int MembershipDurationMonths { get; set; }

    }
}
