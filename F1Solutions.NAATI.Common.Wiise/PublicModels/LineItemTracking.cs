using System;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class LineItemTracking
    {
        [DataMember(Name = "TrackingCategoryID", EmitDefaultValue = false)]
        public Guid? TrackingCategoryID { get; set; }
        [DataMember(Name = "TrackingOptionID", EmitDefaultValue = false)]
        public Guid? TrackingOptionID { get; set; }
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(Name = "Option", EmitDefaultValue = false)]
        public string Option { get; set; }
    }
}
