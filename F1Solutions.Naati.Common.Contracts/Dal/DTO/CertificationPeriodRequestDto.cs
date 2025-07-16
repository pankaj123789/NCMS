namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CertificationPeriodRequestDto
    {
        public int CredentialRequestId { get; set; }
        public int CertificationPeriodId { get; set; }
        public int SkillId { get; set; }
        public string Skill { get; set; }
        public string ExternalName { get; set; }
    }
}