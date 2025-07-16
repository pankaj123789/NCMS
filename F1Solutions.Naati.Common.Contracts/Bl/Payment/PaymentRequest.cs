using System;

namespace F1Solutions.Naati.Common.Contracts.Bl.Payment
{
    public class PaymentRequest : IPaymentRequest
    {
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryMonth { get; set; }
        public string CardVerificationValue { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string CardToken { get; set; }
    }
}
