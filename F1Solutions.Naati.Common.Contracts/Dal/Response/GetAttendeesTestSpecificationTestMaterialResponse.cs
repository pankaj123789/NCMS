using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetAttendeesTestSpecificationTestMaterialResponse
    {
        public IList<AttendeeTestSpecificationTestMaterial> AttendeeTestSpecificationTestMaterialList { get; set; }
        public int AttandanceId { get; set; }
        public int TestSessionId { get; set; }
        public int NaatiNumber { get; set; }
        public bool IsMissingFiles { get; set; }
    }
}