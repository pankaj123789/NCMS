namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class RubricTestComponentContract
    {
        public int Id { get; set; }
        public bool WasAttempted { get; set; }
        public bool Successful { get; set; }
        public int MarkingResultTypeId { get; set; }
        public int? RubricTestComponentResultId { get; set; }
    }
}