namespace Ncms.Contracts
{
    public class CredentialRequestSummarySearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string Filter { get; set; }
    }
}
