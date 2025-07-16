using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetJobExaminersRequest
    {
        public IEnumerable<int> TestAttendanceIds { get; set; }
        public bool IncludeExaminerMarkings { get; set; }
    }
}