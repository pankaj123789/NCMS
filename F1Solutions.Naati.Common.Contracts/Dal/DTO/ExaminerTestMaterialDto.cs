using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ExaminerTestMaterialDto
    {
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public ICollection<PanelMembershipDto> PanelMemberships { get; set; }
    }
}