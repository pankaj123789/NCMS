using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialRequestTestSessionModel
    {
        public int CredentialTestSessionId { get; set; }
        public int TestSessionId { get; set; }
        public string TestLocation { get; set; }
        public string TestLocationState { get; set; }
        public string Name { get; set; }

        public DateTime TestDate { get; set; }
        public bool Rejected { get; set; }
        public bool Completed { get; set; }
        public bool HasAssets { get; set; }
        public bool HasExaminers { get; set; }
        public bool HasPaidReviewExaminers { get; set; }

        public bool Sat { get; set; }

        public int? TestResultStatusId { get; set; }
        public int? TestStatusTypeId { get; set; }
        public int? TestResultId { get; set; }
        public int TotalTestResults { get; set; }
        public bool? TestResultChecked { get; set; }
        public bool? AllowIssue { get; set; }
        public bool Supplementary { get; set; }
        public bool HasDefaultSpecification { get; set; }
        public int TestSpecificationId { get; set; }
        public int MarkingSchemaTypeId { get; set; }
        public bool ShowNonPreferredTestLocationInfo { get; set; }
        public IList<TestSittingMaterialModel> Materials { get; set; }
        public virtual bool? EligibleForConcededPass { get; set; }
        public virtual bool? EligibleForSupplementary { get; set; }
        public bool AutomaticIssuing { get; set; }
        public double? MaxScoreDifference { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime AllocatedDate { get; set; }

    }

    public class TestSittingMaterialModel
    {
        public int TestSittingTestMaterialId { get; set; }

        public int TestTaskId { get; set; }

        public int TestMaterialId { get; set; }

        public int ObjectStatusId { get; set; }

    }
}
