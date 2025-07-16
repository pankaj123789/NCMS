using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    [DataContract]
    public class Contacts
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("Value")]
        public List<Contact> _Contacts { get; set; }

        public Contacts()
        {
            _Contacts = new List<Contact>();
        }
    }
}
