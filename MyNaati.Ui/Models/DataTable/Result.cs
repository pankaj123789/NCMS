using System.Collections.Generic;

namespace MyNaati.Ui.Models.DataTable
{
    public class Result<T>
    {
        private int? mFiltered;

        public int Draw { get; set; }
        public int Total { get; set; }

        public int Filtered
        {
            get
            {
                return mFiltered.HasValue ? mFiltered.Value : Total;
            }

            set
            {
                mFiltered = value;
            }
        }

        public List<T> Data { get; set; }
    }
}
