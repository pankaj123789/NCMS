namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateCountMarksRequest
    {
        public int TestResultId { get; set; }
        public int[] JobExaminersId { get; set; }
        public bool IncludePreviousMarks { get; set; }
    }
}