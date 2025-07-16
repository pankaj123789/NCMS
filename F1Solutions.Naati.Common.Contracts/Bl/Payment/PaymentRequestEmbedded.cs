using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Bl.Payment
{
    public class PaymentRequestEmbedded : IPaymentRequest
    {
        public decimal Amount { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string CreditCardToken { get; set; }
    }
}
