using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class WiiseLineItemModel
    {
        //public string AccountCode { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public bool Gst { get; set; }
        public List<Tuple<string, string>> TrackingCategories { get; set; }
        public Guid? AccountId { get; set; }

    }
}
