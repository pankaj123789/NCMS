namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestSittingRequest
    {
        public int TestSittingId { get; set; }
        public int CredentialRequestId { get; set; }
        public bool Supplementary { get; set; }
    }
}