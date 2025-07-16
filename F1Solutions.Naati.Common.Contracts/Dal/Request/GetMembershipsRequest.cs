using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetMembershipsRequest
    {
        public int PanelId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MembershipId { get; set; }
    }
}