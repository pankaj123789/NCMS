namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationFieldDataDto : CredentialApplicationFieldDto
    {
        public int FieldDataId { get; set; }

        public string Value { get; set; }

        public int FieldOptionId { get; set; }

    }
}
