using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class PublicLegacyAccreditionsResponse :BaseResponse
    {
        public IEnumerable<PublicAccreditationLegacy> Results { get; set; }
    }

    public class PublicAccreditationLegacy
    {
        public string Level { get; set; }
        public string Category { get; set; }
        public string Direction { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
