using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;

namespace MyNaati.Ui.ViewModels
{
    public class PagingData
    {
        public int Page { get; set; }
        public int Rows { get; set; }
        public string Sidx { get; set; }
        public string Sord { get; set; }
        

        public SortDirection SortDirection
        {
            get
            {
                if (Sord == "none")
                {
                    return SortDirection.None;
                }

                return Sord == "Ascending" ? SortDirection.Ascending : SortDirection.Descending;
            }
        }
    }
}