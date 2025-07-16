using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class TestDataRequest
    {
        public int? JobId { get; set; }
        public  int? TestAttendanceId { get; set; }
        public bool CreateTestResult { get; set; }
        public DateTime DueDate { get; set; }
        public bool AllowCalculate { get; set; }
        public bool IncludePreviousMarks { get; set; }
        public int UserId { get; set; }
        public bool EligibleForConcededPass { get; set; }
        public bool EligibleForSupplementary { get; set; }
    }
}