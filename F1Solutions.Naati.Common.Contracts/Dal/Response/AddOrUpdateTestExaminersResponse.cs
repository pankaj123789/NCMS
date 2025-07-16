using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class AddOrUpdateTestExaminersResponse
    {
        public IEnumerable<int> JobIds { get; set; }
        public IEnumerable<int> JobExaminersIds { get; set; }
    }
}