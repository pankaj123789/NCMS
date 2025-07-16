namespace Ncms.Contracts.Models.Application
{
    public class RecertificationModel
    {
        public int Id { get; set; }
        public int CredentialApplicationId { get; set; }
        public int CertificationPeriodId { get; set; }
    }
}