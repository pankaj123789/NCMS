using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCredentialSearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<CredentialSearchCriteria> Filters { get; set; }
    }
}
