namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationFormDto
    {
        public int Id { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public string CredentialApplicationTypeName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FormUserTypeId { get; set; }
        public bool Inactive { get; set; }
    }
}
