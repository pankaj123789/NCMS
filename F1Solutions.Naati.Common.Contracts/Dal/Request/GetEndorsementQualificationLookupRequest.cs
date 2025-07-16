using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetEndorsementQualificationLookupRequest
    {
        public IEnumerable<string> Locations { get; set; }
        public IEnumerable<int> InstitutionNaatiNumbers { get; set; }
    }
}
