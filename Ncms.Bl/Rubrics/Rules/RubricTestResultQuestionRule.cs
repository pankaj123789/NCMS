namespace Ncms.Bl.Rubrics.Rules
{
    public class RubricTestResultQuestionRule : RubricRule
    {
        public int? TestComponentTypeId { get; set; }
        public int MinimumQuestionsAttempted { get; set; }
        public int MinimumQuestionsPassed { get; set; }
    }
}