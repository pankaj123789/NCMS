using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateTestResultRequest
    {
        public TestResultDto TestResult { get; set; }
        public int MaxCommentLength { get; set; }
    }
}