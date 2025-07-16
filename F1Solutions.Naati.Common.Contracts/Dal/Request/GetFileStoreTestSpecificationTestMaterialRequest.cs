using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetFileStoreTestSpecificationTestMaterialRequest
    {
        public List<AttendeeTestSpecificationTestMaterial> AttendeeTestSpecificationTestMaterialList { get; set; }
        public int AttendanceId { get; set; }
        public string TempFileStorePath { get; set; }
    }
}