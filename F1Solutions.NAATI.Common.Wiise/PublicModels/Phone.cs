using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class Phone
    {
        [DataMember(Name = "PhoneType", EmitDefaultValue = false)]
        public PhoneTypeEnum PhoneType { get; set; }
        [DataMember(Name = "PhoneNumber", EmitDefaultValue = false)]
        public string PhoneNumber { get; set; }
        [DataMember(Name = "PhoneAreaCode", EmitDefaultValue = false)]
        public string PhoneAreaCode { get; set; }
        [DataMember(Name = "PhoneCountryCode", EmitDefaultValue = false)]
        public string PhoneCountryCode { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum PhoneTypeEnum
        {
            DEFAULT = 1,
            DDI = 2,
            MOBILE = 3,
            FAX = 4,
            OFFICE = 5
        }
    }
}
