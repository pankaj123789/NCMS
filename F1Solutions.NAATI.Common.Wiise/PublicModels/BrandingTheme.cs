using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    [DataContract]
    public class BrandingTheme
    {

        [DataMember(Name = "Type", EmitDefaultValue = false)]
        public TypeEnum Type { get; set; }
        [DataMember(Name = "BrandingThemeID", EmitDefaultValue = false)]
        public Guid? BrandingThemeID { get; set; }
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Name = "LogoUrl", EmitDefaultValue = false)]
        public string LogoUrl { get; set; }
        [DataMember(Name = "SortOrder", EmitDefaultValue = false)]
        public int? SortOrder { get; set; }
        [DataMember(Name = "CreatedDateUTC", EmitDefaultValue = false)]
        public DateTime? CreatedDateUTC { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum TypeEnum
        {
            INVOICE = 1
        }
    }
}
