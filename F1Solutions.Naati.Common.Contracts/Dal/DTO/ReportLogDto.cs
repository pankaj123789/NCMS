using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ReportLogDto
    {
        public DateTime LogDate { get; set; }
        public string EntityName { get; set; }
        public string Message { get; set; }
        public ReportLogTypeName LogType { get; set; }
    }
}
