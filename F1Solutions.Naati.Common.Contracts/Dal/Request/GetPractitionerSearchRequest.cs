using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetPractitionerSearchRequest : GetPractitionerCountRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<SortingOption> SortingOptions { get; set; }
        public int RandomSeed { get; set; }
    }

    public class GetApiPublicPractitionerSearchRequest : GetAPiPublicPractitionerCountRequest, ISearchPagingRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public IEnumerable<ApiPublicSortingOption> SortingOptions { get; set; }
        public int RandomSeed { get; set; }
    }
}