using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.Certificate
{
    public class SelectCertificateViewModel
    {
        public List<CertificateOrder> Certificates { get; set; }
        public int TotalUnlaminated { get; set; }
        public int TotalLaminated { get; set; }
    }
}