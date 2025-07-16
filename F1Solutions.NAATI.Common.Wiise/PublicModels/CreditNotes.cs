using System.Collections.Generic;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    [DataContract]
    public class CreditNotes
    {
        [DataMember(Name = "CreditNote", EmitDefaultValue = false)]
        public List<CreditNote> _CreditNotes { get; set; }
    }
}
