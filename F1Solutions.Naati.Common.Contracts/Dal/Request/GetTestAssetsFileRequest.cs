namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestAssetsFileRequest
    {
        public int TestSessionCredentialRequestId { get; set; }
        public string TempFileStorePath { get; set; }
        public int TestSittingId { get; set; }
    }
}