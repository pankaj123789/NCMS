using System.Collections.Generic;

namespace Ncms.Contracts.Models.Person
{
    public class DeleteResponseModel
    {
        public bool Success { get; set; }
        public KeyValuePair<string, string> FlowMessage { get; set; }
    }
}
