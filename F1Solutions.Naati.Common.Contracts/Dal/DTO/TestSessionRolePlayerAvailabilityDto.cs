using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionRolePlayerAvailabilityDto : RolePlayerAvailabilityDto
    {
        public int TestSessionRolePlayerId { get; set; }
        public bool Attended { get; set; }
        public bool Rehearsed { get; set; }
        public bool Rejected { get; set; }
        public int RolePlayerStatusId { get; set; }
        public IEnumerable<RolePlayerTaskDetailDto> Details { get; set; }

    }
}