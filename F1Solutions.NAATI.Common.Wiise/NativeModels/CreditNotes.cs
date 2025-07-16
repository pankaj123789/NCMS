using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    [DataContract]
    public class CreditNotes
    {
        [DataMember(Name = "CreditNote", EmitDefaultValue = false)]
        [JsonProperty("Value")]
        public List<CreditNote> _CreditNotes { get; set; }

        public CreditNotes()
        {
            _CreditNotes = new List<CreditNote>();
        }

    }

}
