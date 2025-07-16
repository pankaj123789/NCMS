using Newtonsoft.Json;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class Accounts
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("Value")]
        public List<Account> _Accounts { get; set; }

        public Accounts()
        {
            _Accounts = new List<Account>();
        }
    }
}
