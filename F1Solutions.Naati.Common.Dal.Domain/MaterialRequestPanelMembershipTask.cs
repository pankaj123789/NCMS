namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestPanelMembershipTask : EntityBase
    {
        public virtual MaterialRequestPanelMembership MaterialRequestPanelMembership { get; set; }
        public virtual MaterialRequestTaskType MaterialRequestTaskType { get; set; }
        public virtual double HoursSpent { get; set; }
    }
}
