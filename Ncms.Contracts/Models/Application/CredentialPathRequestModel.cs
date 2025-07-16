using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialPathRequestModel
    {
        public IEnumerable<int> CredentialTypesIdsFrom { get; set; }
    }
}
