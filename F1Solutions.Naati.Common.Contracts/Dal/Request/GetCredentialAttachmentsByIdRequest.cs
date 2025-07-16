namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCredentialAttachmentsByIdRequest
    {
        public int CredentialId { get; set; }
        public int NaatiNumber { get; set; }
    }
}