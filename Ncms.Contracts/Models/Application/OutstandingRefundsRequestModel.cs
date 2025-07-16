using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts.Models.Application
{
    public class OutstandingRefundsRequestModel
    {
        public IEnumerable<string> CreditNotes { get; set; }
    }
}
