namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class DeleteTestAttendanceAssetRequest
    {
        public int TestAttendanceAssetId { get; set; }
        public bool PermanentDelete { get; set; }
    }
}