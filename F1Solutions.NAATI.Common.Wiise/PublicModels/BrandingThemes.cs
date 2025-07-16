using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class BrandingThemes
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("Value")]
        public List<BrandingTheme> _BrandingThemes { get; set; }

        public BrandingThemes()
        {
            _BrandingThemes = new List<BrandingTheme>();
        }
    }
}
