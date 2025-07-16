namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class DeleteAttachmentRequest
    {
        public int TestAttendanceDocumentId { get; set; }
        public int NAATINumber { get; set; }
    }
}