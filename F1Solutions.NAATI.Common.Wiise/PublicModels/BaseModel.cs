using Newtonsoft.Json;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Wiise.PublicModels
{
    public class BaseModel
    {
        [JsonIgnore]
        public bool HasValidationErrors { get; set; }
        [JsonIgnore]
        public List<ValidationError> ValidationErrors { get; set; }

        public BaseModel()
        {
            ValidationErrors = new List<ValidationError>();
        }
    }
}
