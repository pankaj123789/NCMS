using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class AssignPayrollAccountingReferenceRequest
    {
        public int PayrollId { get; set; }
        public IEnumerable<int> JobExaminerIds { get; set; }
        public string AccountingReference { get; set; }
        public int UserId { get; set; }
    }
}