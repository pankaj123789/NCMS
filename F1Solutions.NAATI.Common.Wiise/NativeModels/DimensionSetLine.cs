using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class DimensionSetLine
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("valueCode")]
        public string ValueCode { get; set; }
    }
}
