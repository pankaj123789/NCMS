using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ValidateMarkingForPayrollRequest
    {
        public IEnumerable<int> JobExaminerIds { get; set; }
        public int UserId { get; set; }
    }
}