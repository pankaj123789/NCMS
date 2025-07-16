using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreatePayrollRequest
    {
        public IEnumerable<int> JobExaminerIds { get; set; }
        public bool LegacyAccounting { get; set; }
        public int UserId { get; set; }
    }
}