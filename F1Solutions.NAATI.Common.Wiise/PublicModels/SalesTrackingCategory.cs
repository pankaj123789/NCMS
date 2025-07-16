using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class SalesTrackingCategory
    {

        [DataMember(Name = "TrackingCategoryName", EmitDefaultValue = false)]
        public string TrackingCategoryName { get; set; }
        [DataMember(Name = "TrackingOptionName", EmitDefaultValue = false)]
        public string TrackingOptionName { get; set; }
    }
}
