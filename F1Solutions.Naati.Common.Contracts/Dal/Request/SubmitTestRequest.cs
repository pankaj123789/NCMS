using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SubmitTestRequest
    {
        public int UserId { get; set; }
        public int TestResultID { get; set; }
        public List<StandardTestComponentContract> Components { get; set; }
        public string Comments { get; set; }
        public string Feedback { get; set; }
        public List<string> ReasonsForPoorPerformance { get; set; }
        public int PrimaryReasonForFailure { get; set; }
    }
}