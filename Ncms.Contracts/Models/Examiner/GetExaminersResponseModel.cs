using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Examiner
{
    public class GetExaminersResponseModel
    {
        public ExtendedExaminerModel[] Examiners { get; set; }
    }

    public class JobExaminerModel
    {
        public int JobExaminerId { get; set; }
        
        public int JobId { get; set; }
        public int TestResultId { get; set; }

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

        public int ExaminerStatusId { get; set; }
        public string ExaminerStatusName { get; set; }

        public int ExaminerTypeId { get; set; }
        public string ExaminerTypeName { get; set; }
        public DateTime? ProductSpecificationChangedDate { get; set; }

        public int? ProductSpecificationChangedUserId { get; set; }

        public int? ExaminerSentUserId { get; set; }
        public int? ExaminerReceivedUserId { get; set; }
        public int? ExaminerToPayrollUserId { get; set; }
        public DateTime? ExaminerToPayrollDate { get; set; }

        public PersonExaminerModel Examiner { get; set; }

        public IEnumerable<ExaminerMarkingModel> ExaminerMarkings { get; set; }
    }

    public class ExaminerMarkingModel
    {
        public int JobExaminerId { get; set; }
        public int TestResultId { get; set; }
        public bool CountMarks { get; set; }
        public string Comments { get; set; }
        public string ReasonsForPoorPerformance { get; set; }
        public int PrimaryReasonForFailure { get; set; }
        public string Status { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public IEnumerable<ExaminerTestComponentResultModel> TestComponentResults { get; set; }
    }

    public class ExaminerTestComponentResultModel
    {
        public int ExaminerTestComponentResultId { get; set; }
        public int ExaminerMarkingId { get; set; }
        public double Mark { get; set; }
        public int? TestComponentTypeId { get; set; }
        public int? TotalMarks { get; set; }
        public double? PassMark { get; set; }
        public int? ComponentNumber { get; set; }
        public int? GroupNumber { get; set; }
    }

    public class PersonExaminerModel
    {
        public int PanelMembershipId { get; set; }
        public int EntityId { get; set; }
        public bool IsChair { get; set; }
        public int NaatiNumber { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public DateTime MembershipStartDate { get; set; }
        public DateTime MembershipEndDate { get; set; }
        public string PanelName { get; set; }
    }

    public class ExaminerModel
    {
        public int EntityId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int NAATINumber { get; set; }
        public bool IsChair { get; set; }
        public int PanelId { get; set; }
        public int PanelMembershipId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public bool DoNotSendCorrespondence { get; set; }
    }

    public class ExtendedExaminerModel : ExaminerModel
    {
        public int? TestResultId { get; set; }
        public DateTime? DateAllocated { get; set; }
        public DateTime? DueDate { get; set; }
        public double? ExaminerCost { get; set; }
        public bool? ExaminerPaperLost { get; set; }
        public DateTime? ExaminerPaperReceivedDate { get; set; }
        public DateTime? ExaminerReceivedDate { get; set; }
        public string ExaminerReceivedUser { get; set; }
        public int? ExaminerReceivedUserID { get; set; }
        public DateTime? ExaminerSentDate { get; set; }
        public string ExaminerSentUser { get; set; }
        public int? ExaminerSentUserID { get; set; }
        public DateTime? ExaminerToPayrollDate { get; set; }
        public string ExaminerToPayrollUser { get; set; }
        public int? ExaminerToPayrollUserID { get; set; }
        public int? JobExaminerID { get; set; }
        public int? JobId { get; set; }
        public bool? LetterRecipient { get; set; }
        public string MarkerStatus { get; set; }
        public string NaatiNumberDisplay { get; set; }
        public bool? PaidReviewer { get; set; }
        public bool? ThirdExaminer { get; set; }
        public int PanelRoleId { get; set; }
        public int? ProductSpecificationId { get; set; }
        public string ProductSpecificationCode { get; set; }
        public string PayrollStatusDisplayName { get; set; }
        // round-trip
        public DateTime? ProductSpecificationChangedDate { get; set; }
        // round-trip
        public int? ProductSpecificationChangedUserId { get; set; }
    }
}
