using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class AttendeeTestSpecificationTestMaterial
    {
        public int AttendanceId { get; set; }
        public int CustomerNumber { get; set; }
        public IList<AttendeeTestMaterial> AttendeeTestMaterialList { get; set; }
        public AttendeeTestSpecification AttendeeTestSpecification { get; set; }
    }
}