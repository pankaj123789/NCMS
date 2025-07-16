using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.Products
{
    public class ProductListViewModel
    {
        public List<Product> Fees;

        public List<AvailableProduct> AvailableProducts;
    }

    public class Product
    {
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal OverseasPrice { get; set; }
    }    
}