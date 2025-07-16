using Newtonsoft.Json;
using System;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class Contact : BaseModel
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("graphContactId")]
        public string GraphContactId { get; set; }
        [JsonProperty("contactId")]
        public string ContactId { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("isBlocked", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool? IsBlocked { get; set; }
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("paymentTermsId", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Guid? PaymentTermsId { get; set; }
        [JsonProperty("shipmentMethodId", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Guid? ShipmentMethodId { get; set; }
        [JsonProperty("paymentMethodId", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Guid? PaymentMethodId { get; set; }
        [JsonProperty("lastModifiedDateTime", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public DateTime LastModifiedDateTime { get; set; }
        [JsonProperty("addressLine1")]
        public string AddressLine1 { get; set; }
        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        public Contact(BaseModel baseModel)
        {
            HasValidationErrors = baseModel.HasValidationErrors;
            ValidationErrors = baseModel.ValidationErrors;
        }

        public Contact() { }
    }
}
