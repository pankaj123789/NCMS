using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LineAmountTypes
    {
        Exclusive = 1,
        Inclusive = 2,
        NoTax = 3
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WiiseInvoiceType
    {
        ACCPAY = 1,
        ACCPAYCREDIT = 2,
        APOVERPAYMENT = 3,
        APPREPAYMENT = 4,
        ACCREC = 5,
        ACCRECCREDIT = 6,
        AROVERPAYMENT = 7,
        ARPREPAYMENT = 8
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InvoiceAccountType
    {
        ACCPAY = 1,
        ACCPAYCREDIT = 2,
        APOVERPAYMENT = 3,
        APPREPAYMENT = 4,
        ACCREC = 5,
        ACCRECCREDIT = 6,
        AROVERPAYMENT = 7,
        ARPREPAYMENT = 8
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StatusEnum
    {
        DRAFT = 1,
        OPEN = 2,
        PAID = 3,
        CANCELED = 4
    }

    public enum InvoiceType
    {
        Invoice = 1,
        CreditNote = 2,
        Bill = 3
    }



}
