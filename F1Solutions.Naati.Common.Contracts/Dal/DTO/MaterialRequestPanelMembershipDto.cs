using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestPanelMembershipDto
    {
        public int Id { get; set; }

        public int PanelMemberShipId { get; set; }
        public int NaatiNumber { get; set; }
        public int EntityId { get; set; }

        public IList<MaterialRequestTaskDto> Tasks { get; set; }
        public int MemberTypeId { get; set; }

        public string GivenName { get; set; }
        public string PrimaryEmail { get; set; }

        public MaterialRequestPayrollDto PayRoll { get; set; }


    }
}
