using Newtonsoft.Json;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.NativeModels
{
    public class BaseModel
    {
        public BaseModel()
        {
            ValidationErrors = new List<ValidationError>();
        }
        [JsonIgnore]
        public bool HasValidationErrors { get; set; }
        [JsonIgnore]
        public List<ValidationError> ValidationErrors { get; set; }
    }
}
