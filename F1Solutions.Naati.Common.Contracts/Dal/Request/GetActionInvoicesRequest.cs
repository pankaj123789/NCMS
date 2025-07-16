namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetActionInvoicesRequest
    {
        public int ActionId { get; set; }
        public int? ApplicationId { get; set; }
        public int? CredentialRequestId { get; set; }
    }
}