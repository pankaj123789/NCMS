using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetMarkingsForPayrollRequest
    {
        public string ExaminerNaatiNumber { get; set; }
        public PayrollStatusName[] PayrollStatuses { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}