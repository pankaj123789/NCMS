using System.Collections.Generic;
using MyNaati.Ui.ViewModels.Products;

namespace MyNaati.Ui.ViewModels.Stamp
{    
    public class StampProduct : Product
    {
        public string Type { get; set; }
    }

    public class StampProductListViewModel
    {
        public List<StampProduct> Fees { get; set; }
        public List<AvailableProduct> AvailableProducts { get; set; }
    }
    
}