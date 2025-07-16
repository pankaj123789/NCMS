using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetUpgradeCredentialPathRequest
    {
        public IEnumerable<int> CredentialTypesIdsFrom { get; set; }
    }
}