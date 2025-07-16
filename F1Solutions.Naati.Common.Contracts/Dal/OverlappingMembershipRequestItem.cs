using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class OverlappingMembershipRequestItem
    {
        public int PersonId { get; set; }
        public int PanelId { get; set; }
        public int PanelRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PanelMembershipId { get; set; }
    }
}