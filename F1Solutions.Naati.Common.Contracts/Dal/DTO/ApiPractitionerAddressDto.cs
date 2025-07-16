namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ApiPractitionerAddressDto
    {
        public int AddressId { get; set; }
        public int CountryId { get; set; }
        public string StreetDetails { get; set; }
        public string Country { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public int? StateId { get; set; }
        public string Postcode { get; set; }
        public int OdAddressVisibilityTypeId { get; set; }
        public bool IsPrimaryAddress { get; set; }
    }
}