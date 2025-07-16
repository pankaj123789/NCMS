using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class LineItem : BaseModel
    {
        public string lineType { get; set; }
        public int? sequence { get; set; }
        public Guid? itemId { get; set; }
        public string description { get; set; }
        public int? quantity { get; set; }
        public double? unitPrice { get; set; }
        public double? unitCost { get; set; }
        public string lineDiscountCalculation { get; set; }
        public double? lineDiscountValue { get; set; }
        public bool? taxable { get; set; }
        public string taxGroupId { get; set; }
        public double? lineAmount { get; set; }
        public double? amountExcludingTax { get; set; }
        public double? amountIncludingTax { get; set; }
        public double? invoiceDiscountAmount { get; set; }
        public double? taxPercent { get; set; }
        public double? totalTaxAmount { get; set; }
        public Guid? accountId { get; set; }
        [JsonProperty("dimensionSetLines")]
        public List<DimensionSetLine> DimensionSetLines { get; set; }
        [JsonProperty("shipmentDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? shipmentDate { get; set; }

        public LineItem()
        {
            DimensionSetLines = new List<DimensionSetLine>();
        }

        public bool ShouldSerializeDimensionSetLines()
        {
            return DimensionSetLines.Count > 0;
        }
    }
}
