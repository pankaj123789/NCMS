namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetActiveTestSessionRequest
    {
        public int CredentialApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public int TestLocationId { get; set; }
    }
}