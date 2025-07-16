namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateOrUpdateTestAttendanceAssetRequest
    {
        public int TestAttendanceAssetId { get; set; }
        public int TestAttendanceId { get; set; }
        public int StoredFileId { get; set; }
        public string Title { get; set; }
        public bool Deleted { get; set; }
        public bool EportalDownload { get; set; }
    }
}