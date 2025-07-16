using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.Test
{
    public class TestTaskModel
    {
        public int? TestMaterialId { get; set; }
        
        public int TestComponentId { get; set; }
        [LookupDisplay(LookupType.TestMaterialDomain, "Domain")]
        public int? TestMaterialDomainId { get; set; }
        public string TestComponentName { get; set; }
        public string Label { get; set; }
        public string TestComponentTypeLabel { get; set; }
    }
}
