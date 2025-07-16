using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ExaminerMarkingDto
    {
        public int JobExaminerId { get; set; }
        public int TestResultId { get; set; }
        public bool CountMarks { get; set; }
        public string Comments { get; set; }
        public string ReasonsForPoorPerformance { get; set; }
        public int PrimaryReasonForFailure { get; set; }
        public string Status { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public IEnumerable<ExaminerTestComponentResultDto> TestComponentResults { get; set; }
    }
}