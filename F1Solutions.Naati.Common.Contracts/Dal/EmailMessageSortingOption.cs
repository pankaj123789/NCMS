using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class EmailMessageSortingOption
    {
        public EmailSortType SortType { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}