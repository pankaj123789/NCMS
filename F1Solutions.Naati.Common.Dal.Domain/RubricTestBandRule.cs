using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RubricTestBandRule : EntityBase
    {
        public virtual int TestSpecificationId { get; set; }
        public virtual TestResultEligibilityType TestResultEligibilityType { get; set; }
        public virtual int TestComponentTypeId { get; set; }
        public virtual int RubricMarkingAssessmentCriterionId { get; set; }
        public virtual int MaximumBandLevel { get; set; }
        public virtual int NumberOfQuestions { get; set; }
        public virtual string RuleGroup { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
    }
}