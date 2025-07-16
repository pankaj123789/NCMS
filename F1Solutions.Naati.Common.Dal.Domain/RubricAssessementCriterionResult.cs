using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RubricAssessementCriterionResult: EntityBase
    {
        public virtual RubricTestComponentResult RubricTestComponentResult { get; set; }
        public virtual RubricMarkingAssessmentCriterion RubricMarkingAssessmentCriterion { get; set; }
        public virtual RubricMarkingBand RubricMarkingBand { get; set; }
        public virtual string Comments { get; set; }
        public virtual DateTime? CommentDate { get; set; }
    }
}
