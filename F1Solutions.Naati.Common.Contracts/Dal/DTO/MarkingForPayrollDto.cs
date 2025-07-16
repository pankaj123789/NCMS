using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MarkingForPayrollDto
    {
        public string ProductSpecificationChangedUser { get; set; }
        public string TestType { get; set; }
        public string Language { get; set; }
        public int JobExaminerId { get; set; }
        public string Examiner { get; set; }
        public int ExaminerPersonId { get; set; }
        public int ExaminerEntityId { get; set; }
        public int ExaminerNaatiNumber { get; set; }
        public string ExaminerAccountNumber { get; set; }
        public int TestAttendanceId { get; set; }
        public int ProductSpecificationId { get; set; }
        public string ProductSpecificationCode { get; set; }
        public string GlCode { get; set; }
        public decimal ExaminerCost { get; set; }
        public DateTime ResultReceivedDate { get; set; }
        public DateTime? PaperReceivedDate { get; set; }
        public string Office { get; set; }
        public bool PaidReviewer { get; set; }
        public bool Supplementary { get; set; }
        public int? PayrollId { get; set; }
        public string AccountingReference { get; set; }
        public DateTime? PayrollModifiedDate { get; set; }
        public string PayrollModifiedUser { get; set; }
        public string PayrollStatus { get; set; }
        public string ValidatedUser { get; set; }
        public DateTime? ValidatedDate { get; set; }
    }
}