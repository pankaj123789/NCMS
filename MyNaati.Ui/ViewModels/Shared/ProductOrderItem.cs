using System;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class ProductOrderItem
    {
        public string Product { get; set; }
        public string Skill { get; set; }
        public string Level { get; set; }
        public string Direction { get; set; }
        public DateTime? Expiry { get; set; }
        public int Quantity { get; set; }
        public bool IsAustraliaPricing { get; set; }
        public decimal AustraliaPrice { get; set; }
        public decimal OverseasPrice { get; set; }

        public decimal UnitPrice 
        {
            get
            {
                return IsAustraliaPricing ? AustraliaPrice : OverseasPrice; 
            }
        }

        public decimal TotalPrice 
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }
    }
}