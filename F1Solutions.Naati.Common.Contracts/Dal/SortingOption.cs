using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class SortingOption
    {
        public SortType SortType { get; set; }
        public SortDirection SortDirection { get; set; }
    }

    public class ApiPublicSortingOption
    {
        public ApiPublicSortType SortTypeId { get; set; }
        public ApiPublicSortDirection SortDirectionId { get; set; }
    }
}