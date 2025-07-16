using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class ValidationError
    {
        [DataMember(Name = "Message", EmitDefaultValue = false)]
        public string Message { get; set; }

        public int ErrorCode { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ValidationError {\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
