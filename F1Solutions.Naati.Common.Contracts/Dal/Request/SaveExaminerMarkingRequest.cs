using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveExaminerMarkingRequest
    {
        public JobExaminerMarkingDto JobExaminerMarking { get; set; }
        public bool IsExaminerRequest { get; set; }
        public int MaxCommentsLength { get; set; }
        public bool ClearNotAttempted { get; set; }
    }
}