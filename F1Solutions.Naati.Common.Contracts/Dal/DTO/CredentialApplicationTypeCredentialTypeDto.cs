namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationTypeCredentialTypeDto
    {
        public int Id { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public bool HasTest { get; set; }
        public bool AllowPaidReview { get; set; }
        public bool AllowSupplementary { get; set; }
    }
}
