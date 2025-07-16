using System.Collections.Generic;
using MyNaati.Ui.ViewModels.Products;

namespace MyNaati.Ui.ViewModels.IdCard
{
    public class IdCardListViewModel
    {
        public List<Product> Fees { get; set; }
        public List<IdCard> IdCards { get; set; }
    }

    public class IdCard
    {
        public string Type { get; set; }
    }
}