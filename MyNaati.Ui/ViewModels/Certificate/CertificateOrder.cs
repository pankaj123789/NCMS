using System;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.Certificate
{
    public class CertificateOrder
    {        
        public string Skill { get; set; }
        public string Level { get; set; }
        public string Direction { get; set; } 
        [UIHint("dateOnly")]
        public DateTime? Expiry { get; set; }

        [UIHint("integer")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Quantity of Laminated Certificates must not be negative.")]
        public int QuantityLaminated { get; set; }

        [UIHint("integer")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Quantity of Unlaminated Certificates must not be negative.")]
        public int QuantityUnlaminated { get; set; }
    }
}