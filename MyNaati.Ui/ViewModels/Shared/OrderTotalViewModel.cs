using System.Collections.Generic;
using System.Linq;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class OrderTotalViewModel
    {
        public bool IsIdCard { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ProductOrderItem> Items { get; set; }
        public int TotalQuantity
        {
            get { return Items.Sum(x => x.Quantity); }
        }
        public OrderTotalViewModel()
        {
            IsIdCard = false;
            Items = new List<ProductOrderItem>();
        }
    }
}