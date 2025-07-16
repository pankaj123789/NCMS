using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Bl.Payment
{
    public class GatewayTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        public string Scope { get; set; }
    }
}
