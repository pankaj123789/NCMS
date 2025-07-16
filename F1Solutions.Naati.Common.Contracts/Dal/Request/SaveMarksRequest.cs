namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveMarksRequest
    {
        public int TestResultId { get; set; }
        public StandardTestComponentContract[] Components { get; set; }
    }
}