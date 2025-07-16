namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PersonCredentialRequestDto
    {
        public int PersonId { get; set; }
        public string CredentialType { get; set; }
        public string Direction { get; set; }
        public string CredentialStatus { get; set; }
    }
}
