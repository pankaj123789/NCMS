namespace Ncms.Contracts.Models.Application
{
    public class CredentialApplicationTypeCredentialTypeModel
    {
        public int Id { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public int CredentialTypeId { get; set; }
        public bool HasTest { get; set; }
        public bool AllowSupplementary { get; set; }
        public bool AllowPaidReview { get; set; }
        public bool HasTestFee { get; set; }
    }
}
