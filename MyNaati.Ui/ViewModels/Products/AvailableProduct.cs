using System;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.Products
{
    public class AvailableProduct
    {
        public string Skill { get; set; }
        public string Level { get; set; }
        public string Direction { get; set; }
        [UIHint("dateOnly")]
        public DateTime? Expiry { get; set; }
    }
}