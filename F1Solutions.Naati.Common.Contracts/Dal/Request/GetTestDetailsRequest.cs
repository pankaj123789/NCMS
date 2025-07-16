namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestDetailsRequest
    {
        public int TestResultId { get; set; }
        public int? NaatiNumber { get; set; }
        public bool? UseOriginalResultMark { get; set; }
    }
}