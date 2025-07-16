namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class DowngradedCredentialRequestDto
    {
        public int CredentailTypeId { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public int SkillId { get; set; }
        public string Skill { get; set; }
        public int CategorId { get; set; }
        public int NaatiNumber { get; set; }
        public bool Certification { get; set; }
    }
}