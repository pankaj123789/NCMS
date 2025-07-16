using System.Collections.Generic;
using System.Linq;

namespace MyNaati.Ui.Models.DataTable
{
    public class Parameters
    {
        public int Draw { get; set; }
        public Column[] Columns { get; set; }
        public Order[] Order { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Search Search { get; set; }
        public Dictionary<string, OrderDirection> SortOrder
        {
            get
            {
                return Columns != null && Order != null && Order.Any()
                    ? Order.ToDictionary(x => Columns[x.Column].Data, x => x.OrderDirection)
                    : new Dictionary<string, OrderDirection>();
            }
        }
    }
}
