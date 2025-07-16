using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ReappointMembersRequest
    {
        public int PanelId { get; set; }
        public int[] PanelMembershipNumbers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}