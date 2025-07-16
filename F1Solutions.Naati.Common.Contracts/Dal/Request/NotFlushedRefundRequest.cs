using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class NotFlushedRefundRequest
    {
        public DateTime MaxProcessedDate { get; set; }
        public int Take { get; set; }      

    }
}
