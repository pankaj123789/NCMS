using System;

namespace F1Solutions.Naati.Common.Bl.Payment
{
    public class GatewayPaymentResponse : SecurePayAPIResponse
    {
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string BankTransactionId { get; set; }
        public string ErrorCode { get; set; }
        public string GatewayResponseCode { get; set; }
        public string GatewayResponseMessage { get; set; }
    }
}
