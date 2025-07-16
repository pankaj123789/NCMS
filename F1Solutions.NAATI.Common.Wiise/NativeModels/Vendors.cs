using Newtonsoft.Json;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    internal class Vendors:BaseModel
    {
        [JsonProperty("@odata.context")]
        internal string OdataContext { get; set; }
        [JsonProperty("Value")]
        internal List<Vendor> _Vendors { get; set; }

        internal Vendors()
        {
            _Vendors = new List<Vendor>();
        }
    }
}
