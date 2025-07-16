using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
using MyNaati.Ui.Models.DataTable;

namespace MyNaati.Ui.Helpers
{
    public static class DataTableExtensions
    {
        public static object ToObject<T>(this Result<T> result)
        {
            return new
            {
                draw = result.Draw,
                recordsTotal = result.Total,
                recordsFiltered = result.Filtered,
                data = result.Data
            };
        }

        public static Dictionary<string, SortDirection> GetSortDirection(this Parameters parameters)
        {
            return parameters.SortOrder.ToDictionary(x => x.Key, x => x.Value == OrderDirection.Descending ? SortDirection.Descending : SortDirection.Ascending);
        }
    }
}
