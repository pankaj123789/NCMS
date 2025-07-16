using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestPanelMembershipModel
    {
        public int Id { get; set; }

        [LookupDisplay(LookupType.MaterialRequestRoundMembershipType, "MemberTypeName")]
        public  int MemberTypeId { get; set; }
        public int PanelMemberShipId { get; set; }
        public int NaatiNumber { get; set; }

        public int EntityId { get; set; }

        public string GivenName { get; set; }
        public string PrimaryEmail { get; set; }
        public IList<MaterialRequestTaskModel> Tasks { get; set; }

        public MaterialRequestPayrollModel PayRoll { get; set; }

        public double TotalHours => Tasks.Sum(x => x.HoursSpent);

    }
}
