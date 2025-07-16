using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class LineItem
    {
        [DataMember(Name = "UnitAmount", EmitDefaultValue = false)]
        public decimal? UnitAmount { get; set; }
        [DataMember(Name = "DiscountRate", EmitDefaultValue = false)]
        public decimal? DiscountRate { get; set; }
        [DataMember(Name = "LineAmount", EmitDefaultValue = false)]
        public decimal? LineAmount { get; set; }
        [DataMember(Name = "TaxAmount", EmitDefaultValue = false)]
        public decimal? TaxAmount { get; set; }
        [DataMember(Name = "TaxType", EmitDefaultValue = false)]
        public string TaxType { get; set; }
        //[DataMember(Name = "AccountCode", EmitDefaultValue = false)]
        //public string AccountCode { get; set; }
        [DataMember(Name = "ItemCode", EmitDefaultValue = false)]
        public bool Gst { get; set; }
        [DataMember(Name = "Gst", EmitDefaultValue = false)]
        public string ItemCode { get; set; }
        [DataMember(Name = "RepeatingInvoiceID", EmitDefaultValue = false)]
        public Guid? RepeatingInvoiceID { get; set; }
        [DataMember(Name = "Quantity", EmitDefaultValue = false)]
        public decimal? Quantity { get; set; }
        [DataMember(Name = "Description", EmitDefaultValue = false)]
        public string Description { get; set; }
        [DataMember(Name = "LineItemID", EmitDefaultValue = false)]
        public Guid? LineItemID { get; set; }
        [DataMember(Name = "DiscountAmount", EmitDefaultValue = false)]
        public decimal? DiscountAmount { get; set; }
        public Guid AccountId { get; set; }
        [DataMember(Name = "Tracking", EmitDefaultValue = false)]
        public List<LineItemTracking> Tracking { get; set; }
    }
}
