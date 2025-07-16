using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RubricTestComponentResult : EntityBase
    {
        private IList<RubricAssessementCriterionResult> mRubricAssessementCriterionResults = new List<RubricAssessementCriterionResult>();
        public virtual IEnumerable<RubricAssessementCriterionResult> RubricAssessementCriterionResults =>
            mRubricAssessementCriterionResults;
        public virtual TestComponent TestComponent { get; set; }
        public virtual bool WasAttempted { get; set; }
        public virtual bool? Successful { get; set; }

        public virtual MarkingResultType MarkingResultType { get; set; }

    } 
}
