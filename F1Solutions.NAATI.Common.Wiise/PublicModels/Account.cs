using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class Account
    {
        [DataMember(Name = "AccountID", EmitDefaultValue = false)]
        public Guid? AccountID { get; set; }
        [DataMember(Name = "HasAttachments", EmitDefaultValue = false)]
        public bool? HasAttachments { get; }
        [DataMember(Name = "ReportingCodeName", EmitDefaultValue = false)]
        public string ReportingCodeName { get; }
        [DataMember(Name = "ReportingCode", EmitDefaultValue = false)]
        public string ReportingCode { get; }
        [DataMember(Name = "ShowInExpenseClaims", EmitDefaultValue = false)]
        public bool? ShowInExpenseClaims { get; set; }
        [DataMember(Name = "EnablePaymentsToAccount", EmitDefaultValue = false)]
        public bool? EnablePaymentsToAccount { get; set; }
        [DataMember(Name = "TaxType", EmitDefaultValue = false)]
        public string TaxType { get; set; }
        [DataMember(Name = "Description", EmitDefaultValue = false)]
        public string Description { get; set; }
        [DataMember(Name = "BankAccountNumber", EmitDefaultValue = false)]
        public string BankAccountNumber { get; set; }
        [DataMember(Name = "ValidationErrors", EmitDefaultValue = false)]
        public List<ValidationError> ValidationErrors { get; set; }
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Name = "Code", EmitDefaultValue = false)]
        public string Code { get; set; }
        [DataMember(Name = "SystemAccount", EmitDefaultValue = false)]
        public SystemAccountEnum SystemAccount { get; set; }
        [DataMember(Name = "Class", EmitDefaultValue = false)]
        public ClassEnum Class { get; set; }
        //[DataMember(Name = "CurrencyCode", EmitDefaultValue = false)]
        //public CurrencyCode CurrencyCode { get; set; }
        [DataMember(Name = "BankAccountType", EmitDefaultValue = false)]
        public BankAccountTypeEnum BankAccountType { get; set; }
        [DataMember(Name = "Status", EmitDefaultValue = false)]
        public StatusEnum Status { get; set; }
        //[DataMember(Name = "Type", EmitDefaultValue = false)]
        //public AccountType Type { get; set; }
        [DataMember(Name = "UpdatedDateUTC", EmitDefaultValue = false)]
        public DateTime? UpdatedDateUTC { get; }
        [DataMember(Name = "AddToWatchlist", EmitDefaultValue = false)]
        public bool? AddToWatchlist { get; set; }
        public string Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum StatusEnum
        {
            ACTIVE = 1,
            ARCHIVED = 2,
            DELETED = 3
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum BankAccountTypeEnum
        {
            BANK = 1,
            CREDITCARD = 2,
            PAYPAL = 3,
            NONE = 4,
            Empty = 5
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ClassEnum
        {
            ASSET = 1,
            EQUITY = 2,
            EXPENSE = 3,
            LIABILITY = 4,
            REVENUE = 5
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum SystemAccountEnum
        {
            DEBTORS = 1,
            CREDITORS = 2,
            BANKCURRENCYGAIN = 3,
            GST = 4,
            GSTONIMPORTS = 5,
            HISTORICAL = 6,
            REALISEDCURRENCYGAIN = 7,
            RETAINEDEARNINGS = 8,
            ROUNDING = 9,
            TRACKINGTRANSFERS = 10,
            UNPAIDEXPCLM = 11,
            UNREALISEDCURRENCYGAIN = 12,
            WAGEPAYABLES = 13,
            CISASSETS = 14,
            CISASSET = 15,
            CISLABOUR = 16,
            CISLABOUREXPENSE = 17,
            CISLABOURINCOME = 18,
            CISLIABILITY = 19,
            CISMATERIALS = 20,
            Empty = 21
        }
    }
}

