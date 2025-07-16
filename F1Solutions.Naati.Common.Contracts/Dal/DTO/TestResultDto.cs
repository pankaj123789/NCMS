using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestResultDto
    {
        public int TestResultId { get; set; }
        public int CurrentJobId { get; set; }
        public int ResultTypeId { get; set; }
        public bool ResultChecked { get; set; }
        public bool AllowCalculate { get; set; }
        public bool IncludePreviousMarks { get; set; }
        public string Comments1 { get; set; }
        public string Comments2 { get; set; }
        public string CommentsEthics { get; set; }
        public string CommentsGeneral { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime? SatDate { get; set; }
        public bool ThirdExaminerRequired { get; set; }
        public DateTime? DueDate { get; set; }
        public bool AllowIssue { get; set; }
        public bool EligibleForConcededPass { get; set; }
        public bool EligibleForSupplementary { get; set; }
        public bool? AutomaticIssuingExaminer { get; set; }
    }
}