using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class EntityRecordsResponse
    {
        public IEnumerable<string> PropertyNames { get; set; }
        public IEnumerable<IList<string>> PropertyValues { get; set; }
    }
}