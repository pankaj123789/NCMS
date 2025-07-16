namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestAttendanceDocumentRequest
    {
        public int TestAttendanceDocumentId { get; set; }
        public string TempFileStorePath { get; set; }
    }
}