using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveTestMarkingRequest
    {
        public TestResultMarkingDto TestResultMarking { get; set; }
        public int UserId { get; set; }
        public bool ClearNotAttempted { get; set; }
    }
}