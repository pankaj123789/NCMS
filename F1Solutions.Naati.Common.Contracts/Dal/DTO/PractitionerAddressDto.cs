using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PractitionerAddressDto
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

    public class ApiPublicPractitionerAddressDto
    {
        public string Postcode { get; set; }
        public string State { get; set; }
        public string StreetDetails { get; set; }
        public string Country { get; set; }
        public string Suburb { get; set; }
        [JsonIgnore]
        public bool IsPrimaryAddress { get; set; }
        [JsonIgnore]
        public int OdAddressVisibilityTypeId { get; set; }
        [JsonIgnore]
        public int AddressId { get; set; }
    }

    public class ApiPublicContactDetailsDto
    {
        public string Type { get; set; }
        public string Contact { get; set; }
        [JsonIgnore]
        public string PhonesInPd { get; set; }
        [JsonIgnore]
        public string EmailsInPd { get; set; }
        [JsonIgnore]
        public string WebsiteUrlInPd { get; set; }
    }
}