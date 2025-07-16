using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class Contact
    {
        [DataMember(Name = "Phones", EmitDefaultValue = false)]
        public List<Phone> Phones { get; set; }
        [DataMember(Name = "SalesDefaultAccountCode", EmitDefaultValue = false)]
        public string SalesDefaultAccountCode { get; set; }
        [DataMember(Name = "PurchasesDefaultAccountCode", EmitDefaultValue = false)]
        public string PurchasesDefaultAccountCode { get; set; }
        [DataMember(Name = "SalesTrackingCategories", EmitDefaultValue = false)]
        public List<SalesTrackingCategory> SalesTrackingCategories { get; set; }
        [DataMember(Name = "PurchasesTrackingCategories", EmitDefaultValue = false)]
        public List<SalesTrackingCategory> PurchasesTrackingCategories { get; set; }
        [DataMember(Name = "TrackingCategoryName", EmitDefaultValue = false)]
        public string TrackingCategoryName { get; set; }
        [DataMember(Name = "TrackingCategoryOption", EmitDefaultValue = false)]
        public string TrackingCategoryOption { get; set; }
        [DataMember(Name = "PaymentTerms", EmitDefaultValue = false)]
        public DateTime? UpdatedDateUTC { get; }
        [DataMember(Name = "ContactGroups", EmitDefaultValue = false)]
        public string Website { get; }
        public decimal? Discount { get; }
        [DataMember(Name = "HasAttachments", EmitDefaultValue = false)]
        public bool? HasAttachments { get; set; }
        [DataMember(Name = "IsSupplier", EmitDefaultValue = false)]
        public bool? IsSupplier { get; set; }
        [DataMember(Name = "StatusAttributeString", EmitDefaultValue = false)]
        public string StatusAttributeString { get; set; }
        [DataMember(Name = "Addresses", EmitDefaultValue = false)]
        public List<Address> Addresses { get; set; }
        [DataMember(Name = "AccountsPayableTaxType", EmitDefaultValue = false)]
        public string AccountsPayableTaxType { get; set; }
        [DataMember(Name = "ContactStatus", EmitDefaultValue = false)]
        public ContactStatusEnum ContactStatus { get; set; }
        [DataMember(Name = "ContactID", EmitDefaultValue = false)]
        public Guid? ContactID { get; set; }
        [DataMember(Name = "ContactNumber", EmitDefaultValue = false)]
        public string ContactNumber { get; set; }
        [DataMember(Name = "ValidationErrors", EmitDefaultValue = false)]
        public List<ValidationError> ValidationErrors { get; set; }
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Name = "FirstName", EmitDefaultValue = false)]
        public string FirstName { get; set; }
        [DataMember(Name = "AccountNumber", EmitDefaultValue = false)]
        public string AccountNumber { get; set; }
        [DataMember(Name = "EmailAddress", EmitDefaultValue = false)]
        public string EmailAddress { get; set; }
        [DataMember(Name = "SkypeUserName", EmitDefaultValue = false)]
        public string SkypeUserName { get; set; }
        [DataMember(Name = "BankAccountDetails", EmitDefaultValue = false)]
        public string BankAccountDetails { get; set; }
        [DataMember(Name = "TaxNumber", EmitDefaultValue = false)]
        public string TaxNumber { get; set; }
        [DataMember(Name = "AccountsReceivableTaxType", EmitDefaultValue = false)]
        public string AccountsReceivableTaxType { get; set; }
        [DataMember(Name = "LastName", EmitDefaultValue = false)]
        public string LastName { get; set; }
        [DataMember(Name = "HasValidationErrors", EmitDefaultValue = false)]
        public bool HasValidationErrors { get; set; }
        public int Balances { get; set; }

        public enum ContactStatusEnum
        {
            ACTIVE = 1,
            ARCHIVED = 2,
            GDPRREQUEST = 3
        }

        public Contact(BaseModel baseModel)
        {
            HasValidationErrors = baseModel.HasValidationErrors;
            ValidationErrors = baseModel.ValidationErrors;
        }

        public Contact() { }
    }
}
