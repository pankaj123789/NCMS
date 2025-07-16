using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateMaterialRequestMembersRequest
    {
        public int MaterialRequestId { get; set; }
        public IEnumerable<MaterialRequestPanelMembershipDto> Members { get; set; }
    }
}
