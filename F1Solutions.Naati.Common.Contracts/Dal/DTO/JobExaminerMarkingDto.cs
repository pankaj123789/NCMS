using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class JobExaminerMarkingDto
    {
        public int TestSpecificationId { get; set; }
        public int JobExaminerId { get; set; }
        public int NaatiNumber { get; set; }
        public int AttendanceId { get; set; }
        public int TestResultId { get; set; }
        public int TestResultStatusTypeId { get; set; }
        public string ExaminerName { get; set; }
        public DateTime? SubmittedDate  { get; set; }
        public bool ResultAutoCalculation { get; set; }
        public IEnumerable<TestMarkingComponentDto> TestComponents { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string Feedback { get; set; }
    }
}