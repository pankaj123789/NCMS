using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class LanguagesRequest
    {
        public IEnumerable<int> CredentialTypeIds { get; set; }
    }
}
