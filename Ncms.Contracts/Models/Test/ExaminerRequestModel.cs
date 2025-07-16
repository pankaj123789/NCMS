using System;

namespace Ncms.Contracts.Models.Test
{
    public class ExaminerRequestModel
    {
        public int? JobExaminerId { get; set; }
        public virtual int PanelMembershipId { get; set; }
        public double ExaminerCost { get; set; }
        public bool ThirdExaminer { get; set; }
        public bool PaidReviewer { get; set; }
        public bool ExaminerPaperLost { get; set; }
        public DateTime? ExaminerSentDate { get; set; }
        public DateTime? ExaminerPaperReceivedDate { get; set; }
        public DateTime? ExaminerToPayrollDate { get; set; }
        public DateTime? ExaminerReceivedDate { get; set; }
        public int? ProductSpecificationId { get; set; }
        public bool ProductSpecificationChanged { get; set; }
        public bool ExaminerSentDateChanged { get; set; }
        public bool ExaminerReceivedDateChanged { get; set; }
        public bool ExaminerToPayrollDateChanged { get; set; }
        // round-trip
        public DateTime? ProductSpecificationChangedDate { get; set; }
        // round-trip
        public int? ProductSpecificationChangedUserId { get; set; }
        public int? ExaminerSentUserId { get; set; }
        public int? ExaminerReceivedUserId { get; set; }
        public int? ExaminerToPayrollUserId { get; set; }
    }
}
