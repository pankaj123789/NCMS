namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CredentialTypeResponse
    {
        public int CredentialTypeId { get; set; }
        public int SkillId { get; set; }
        public string InternalName { get; set; }
        public string DisplayName { get; set; }

        public bool? Certification { get; set; }
    }
}