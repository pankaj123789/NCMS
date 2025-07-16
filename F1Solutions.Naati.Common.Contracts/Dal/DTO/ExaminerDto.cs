using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ExaminerDto
    {
        public int EntityId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int NaatiNumber { get; set; }
        public int LanguageId { get; set; }
        public bool IsChair { get; set; }
        public int? TestResultId { get; set; }
        public DateTime? DateAllocated { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double? ExaminerCost { get; set; }
        public bool? ExaminerPaperLost { get; set; }
        public DateTime? ExaminerPaperReceivedDate { get; set; }
        public DateTime? ExaminerReceivedDate { get; set; }
        public string ExaminerReceivedUser { get; set; }
        public int? ExaminerReceivedUserId { get; set; }
        public DateTime? ExaminerSentDate { get; set; }
        public string ExaminerSentUser { get; set; }
        public int? ExaminerSentUserId { get; set; }
        public DateTime? ExaminerToPayrollDate { get; set; }
        public string ExaminerToPayrollUser { get; set; }
        public int? ExaminerToPayrollUserId { get; set; }
        public int? JobExaminerId { get; set; }
        public int? JobId { get; set; }
        public bool? LetterRecipient { get; set; }
        public string MarkerStatus { get; set; }
        public string NaatiNumberDisplay { get; set; }
        public bool? PaidReviewer { get; set; }
        public int PanelId { get; set; }
        public int PanelMembershipId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public bool? ThirdExaminer { get; set; }
        public int PanelRoleId { get; set; }
        public int? ProductSpecificationId { get; set; }
        public string ProductSpecificationCode { get; set; }
        public string PayrollStatusName { get; set; }
        public DateTime? ProductSpecificationChangedDate { get; set; }
        public int? ProductSpecificationChangedUserId { get; set; }
        public bool? PrimaryContact { get; set; }
        public bool DoNotSendCorrespondence { get; set; }
    }
}