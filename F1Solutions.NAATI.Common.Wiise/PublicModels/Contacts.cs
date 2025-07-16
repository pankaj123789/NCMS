using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    [DataContract]
    public class Contacts
    {
        [JsonProperty("Value")]
        public List<Contact> _Contacts { get; set; }

        public Contacts()
        {
            _Contacts = new List<Contact>();
        }
    }
}
