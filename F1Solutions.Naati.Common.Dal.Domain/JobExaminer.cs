using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class JobExaminer : EntityBase
    {
        private IEnumerable<JobExaminerPayroll> mPayRolls = Enumerable.Empty<JobExaminerPayroll>();
        private IEnumerable<ExaminerMarking> mMarkings = Enumerable.Empty<ExaminerMarking>();
        private IList<JobExaminerRubricTestComponentResult> mJobExaminerRubricTestComponentResults = new List<JobExaminerRubricTestComponentResult>();

        public virtual Job Job { get; set; }
        public virtual PanelMembership PanelMembership { get; set; }
        public virtual bool ThirdExaminer { get; set; }
        public virtual bool PaidReviewer { get; set; }
        public virtual DateTime? ExaminerSentDate { get; set; }
        public virtual User ExaminerSentUser { get; set; }
        public virtual DateTime? ExaminerReceivedDate { get; set; }
        public virtual User ExaminerReceivedUser { get; set; }
        public virtual DateTime? ExaminerToPayrollDate { get; set; }
        public virtual User ExaminerToPayrollUser { get; set; }
        public virtual Decimal? ExaminerCost { get; set; }
        public virtual bool ExaminerPaperLost { get; set; }
        public virtual DateTime? ExaminerPaperReceivedDate { get; set; }
        public virtual bool LetterRecipient { get; set; }
        public virtual DateTime DateAllocated { get; set; }
        public virtual string Status { get; set; }
        public virtual ProductSpecification ProductSpecification { get; set; }
        public virtual DateTime? ProductSpecificationChangedDate { get; set; }
        public virtual User ProductSpecificationChangedUser { get; set; }
        public virtual DateTime? ValidatedDate { get; set; }
        public virtual User ValidatedUser { get; set; }
        public virtual bool? PrimaryContact { get; set; }
        public virtual string PayrollStatusName { get; set; }
        public virtual string Feedback { get; set; }

        public virtual IEnumerable<JobExaminerPayroll> PayRolls => mPayRolls;
        public virtual IEnumerable<ExaminerMarking> Markings => mMarkings;

        public virtual IEnumerable<JobExaminerRubricTestComponentResult> JobExaminerRubricTestComponentResults =>
            mJobExaminerRubricTestComponentResults;

        public virtual IEnumerable<RubricTestComponentResult> RubricTestComponentResults => JobExaminerRubricTestComponentResults
            .Select(x => x.RubricTestComponentResult);


    }

    public static class JobExaminerStatus
    {
        public const string Initial = "I";
        public const string Submitted = "S";
    }
}
