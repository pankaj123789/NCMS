using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SetPayrollStatusRequest
    {
        public int PayrollId { get; set; }
        public PayrollStatusName Status { get; set; }
        public int UserId { get; set; }
    }
}