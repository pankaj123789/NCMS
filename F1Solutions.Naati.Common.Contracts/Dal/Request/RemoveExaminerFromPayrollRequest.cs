using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class RemoveExaminerFromPayrollRequest
    {
        public int PayrollId { get; set; }
        public IEnumerable<int> JobExaminerIds { get; set; }
    }
}