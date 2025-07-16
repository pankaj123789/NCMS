namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class MoveCredentialRequest
    {
        public int CredentialId { get; set; }
        public int CertificationPeriodId { get; set; }
        public int UserId { get; set; }
        public string Notes { get; set; }
    }
}