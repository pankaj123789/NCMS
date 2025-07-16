using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
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
