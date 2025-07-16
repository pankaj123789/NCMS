using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PersonExaminerDto
    {
        public int PanelMembershipId { get; set; }
        public int EntityId { get; set; }
        public bool IsChair { get; set; }
        public int NaatiNumber { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public DateTime MembershipStartDate { get; set; }
        public DateTime MembershipEndDate { get; set; }
        public string PanelName { get; set; }
    }
}