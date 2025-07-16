namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class AddressDto
    {
        public string StreetDetails { get; set; }
        public int? PostcodeId { get; set; }
        public int? CountryId { get; set; }
        public string Note { get; set; }
    }
}
