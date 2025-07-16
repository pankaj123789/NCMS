using System.Collections.Generic;
using System.Data;

namespace Ncms.Ui.Models.Api
{
    public class BackgroundTableGetResponse
    {
        public IEnumerable<DataColumn> Columns { get; set; }
        public DataTable Data { get; set; }
    }
}