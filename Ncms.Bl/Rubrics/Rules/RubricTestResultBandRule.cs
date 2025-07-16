namespace Ncms.Bl.Rubrics.Rules
{
    public class RubricTestResultBandRule : RubricMaximumBandRule
    {
        public int TestComponentTypeId { get; set; }
        public int NumberOfQuestions { get; set; }
    }
}