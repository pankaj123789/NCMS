using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestStatus : EntityBase
    {
        public virtual TestStatusType TestStatusType { get; set; }
        public virtual TestSitting TestSitting { get; set; }
        public virtual bool AllMarksReceived { get; set; }
        public virtual bool HasAssets { get; set; }
        public virtual bool HasExaminers { get; set; }
        public virtual bool HasOverdueExaminers { get; set; }
        public virtual bool HasSubmittedExaminers { get; set; }
        public virtual bool HasInProgressExaminers { get; set; }
        public virtual bool HasPaidReviewExaminers { get; set; }
        public virtual bool EligibleForSupplementary { get; set; }
        public virtual bool EligibleForConcededPass { get; set; }
        public virtual bool AllowIssue { get; set; }
        public virtual DateTime? LastExaminerReceivedDate { get; set; }
    }
}

