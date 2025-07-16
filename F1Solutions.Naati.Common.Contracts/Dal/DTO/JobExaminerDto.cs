using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class JobExaminerDto
    {
        public int JobExaminerId { get; set; }
        public  int TestResultId { get; set; }
        public int JobId { get; set; }
        public DateTime DateAllocated { get; set; }
        public DateTime? JobDueDate { get; set; }
        public DateTime? ExaminerReceivedDate { get; set; }
        public DateTime? ExaminerSentDate { get; set; }
        public int? PayRollStatusId { get; set; }
        public string PayRollStatus { get; set; }
        public int ProductSpecificationId { get; set; }
        public string ProductSpecificationCode { get; set; }
        public decimal? ExaminerCost { get; set; }
        public bool PaidReviewer { get; set; }
        public bool ThirdExaminer { get; set; }
        public DateTime? ExaminerPaperReceivedDate { get; set; }
        public bool ExaminerPaperLost { get; set; }
        public DateTime? ProductSpecificationChangedDate { get; set; }
        public int? ProductSpecificationChangedUserId { get; set; }
        public int? ExaminerSentUserId { get; set; }
        public int? ExaminerReceivedUserId { get; set; }
        public int? ExaminerToPayrollUserId { get; set; }
        public DateTime? ExaminerToPayrollDate { get; set; }
        public PersonExaminerDto Examiner { get; set; }
        public IEnumerable<ExaminerMarkingDto> ExaminerMarkings { get; set; }
    }
}