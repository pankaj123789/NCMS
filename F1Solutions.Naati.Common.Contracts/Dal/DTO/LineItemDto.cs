using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class LineItemDto
    {
        //public string AccountCode { get; set; }
        public Guid AccountId { get; set; }
        public string Description { get; set; }
        public decimal? LineAmount { get; set; }
    }
}
