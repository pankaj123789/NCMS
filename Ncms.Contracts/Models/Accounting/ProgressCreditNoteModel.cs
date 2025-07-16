using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts.Models.Accounting
{
    public class ProgressCreditNoteModel
    {
        public int ApplicationId { get; set; }
        public string PaymentReference { get; set; }
        public string CreditNoteNo { get; set; }
    }
}
