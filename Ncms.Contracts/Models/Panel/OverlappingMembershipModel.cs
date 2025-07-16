using System;

namespace Ncms.Contracts.Models.Panel
{
    public class OverlappingMembershipModel
    {
        public int PersonId { get; set; }
        public int PanelId { get; set; }
        public int PanelRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PanelMembershipId { get; set; }
    }

    public class OverlappingRequestModel
    {
        public OverlappingMembershipModel[] Items { get; set; }
    }

}
