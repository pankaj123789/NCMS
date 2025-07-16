using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ExaminerRequest
    {
        public int? JobExaminerId { get; set; }
        public int PanelMemberShipId { get; set; }
        public bool ThirdExaminer { get; set; }
        public DateTime? ExaminerSentDate { get; set; }
        public int? ExaminerSentUserId { get; set; }
        public DateTime? ExaminerReceivedDate { get; set; }
        public int? ExaminerReceivedUserId{ get; set; }
        public DateTime? ExaminerToPayrollDate { get; set; }
        public int? ExaminerToPayrollUserId { get; set; }
        public double ExaminerCost { get; set; }
        public bool ExaminerPaperLost { get; set; }
        public bool LetterRecipient { get; set; }
        public DateTime? ExaminerPaperReceivedDate { get; set; }
        public bool PaidReviewer { get; set; }
        public int? ProductSpecificationId { get; set; }
        public DateTime? ProductSpecificationChangedDate { get; set; }
        public int? ProductSpecificationChangedUserId{ get; set; }
        public DateTime? ValidatedDate { get; set; }
        public int? ValidateUserId { get; set; }
        public bool PrimaryContact { get; set; }
    }
}