using System;

namespace Ncms.Contracts.Models.Panel
{
    public class ReappointMembersModel
    {
        public int PanelId { get; set; }
        public int[] PanelMembershipNumbers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
