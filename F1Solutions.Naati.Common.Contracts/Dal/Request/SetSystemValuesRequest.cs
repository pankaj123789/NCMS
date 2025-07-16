using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SetSystemValuesRequest
    {
        public IDictionary<string, string> Values { get; set; }
    }
}