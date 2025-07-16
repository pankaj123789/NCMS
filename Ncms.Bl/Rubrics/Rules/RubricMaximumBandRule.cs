namespace Ncms.Bl.Rubrics.Rules
{
    public class RubricMaximumBandRule : RubricRule
    {
        public int RubricMarkingAssessmentCriterionId { get; set; }
        public int MaximumBandLevel { get; set; }
    }
}