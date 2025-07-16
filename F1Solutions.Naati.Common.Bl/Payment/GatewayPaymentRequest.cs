using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Bl.Payment
{
    public class GatewayPaymentRequest
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("merchantCode")]
        public string MerchantCode { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("ip")]
        public string IP { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
    }
}
