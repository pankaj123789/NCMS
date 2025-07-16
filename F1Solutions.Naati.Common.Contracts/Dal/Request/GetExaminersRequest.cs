namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetExaminersRequest
    {
        public int[] LanguageId { get; set; }
        public bool? ActiveExaminersOnly { get; set; }
        public int[] PanelId { get; set; }
        public bool JoinJobExaminer { get; set; }
        public int[] JobId { get; set; }
        public int[] JobExaminerId { get; set; }
        public bool JoinTestResult { get; set; }
        public int[] TestAttendanceId { get; set; }
    }
}