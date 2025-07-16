using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CredentialSearchCriteria : ISearchCriteria<CredentialFilterTypeName>
    {
        public CredentialFilterTypeName Filter { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}
