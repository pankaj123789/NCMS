namespace Ncms.Contracts.Models.Panel
{
    public class PanelMembershipInfoModel
    {
        public int PanelMemberShipId { get; set; }
        public int NaatiNumber { get; set; }
        public string GivenName { get; set; }
        public string PrimaryEmail { get; set; }
        public int EntityId { get; set; }
    }
}
