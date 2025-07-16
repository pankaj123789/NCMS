using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestTaskModel
    {
        public int Id { get; set; }
        [LookupDisplay(LookupType.MaterialRequestTaskType, "MaterialRequestTaskTypeName")]
        public int MaterialRequestTaskTypeId { get; set; }
        public double HoursSpent { get; set; }
    }
}
