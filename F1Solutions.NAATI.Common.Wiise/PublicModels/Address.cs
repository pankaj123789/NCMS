using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class Address
    {
        [DataMember(Name = "AddressLine2", EmitDefaultValue = false)]
        public string AddressLine2 { get; set; }
        [DataMember(Name = "AddressType", EmitDefaultValue = false)]
        public AddressTypeEnum AddressType { get; set; }
        [DataMember(Name = "AddressLine1", EmitDefaultValue = false)]
        public string AddressLine1 { get; set; }
        [DataMember(Name = "AttentionTo", EmitDefaultValue = false)]
        public string AttentionTo { get; set; }
        [DataMember(Name = "AddressLine3", EmitDefaultValue = false)]
        public string AddressLine3 { get; set; }
        [DataMember(Name = "AddressLine4", EmitDefaultValue = false)]
        public string AddressLine4 { get; set; }
        [DataMember(Name = "City", EmitDefaultValue = false)]
        public string City { get; set; }
        [DataMember(Name = "Region", EmitDefaultValue = false)]
        public string Region { get; set; }
        [DataMember(Name = "PostalCode", EmitDefaultValue = false)]
        public string PostalCode { get; set; }
        [DataMember(Name = "Country", EmitDefaultValue = false)]
        public string Country { get; set; }
        public string CountryCode { get; set; }


        [JsonConverter(typeof(StringEnumConverter))]
        public enum AddressTypeEnum
        {
            POBOX = 1,
            STREET = 2,
            DELIVERY = 3
        }
    }
}
