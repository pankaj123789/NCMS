using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SearchRequest
    {
        public string Term { get; set; }
        public EntitySearchType Type { get; set; }
    }
}