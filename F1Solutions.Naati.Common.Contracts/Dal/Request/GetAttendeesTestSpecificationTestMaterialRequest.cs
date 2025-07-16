using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetAttendeesTestSpecificationTestMaterialRequest
    {
        public IList<CustomerAttendanceIdDto> CustomerAttendanceIdList { get; set; }
        public bool IncludeExaminer { get; set; }
    }
}