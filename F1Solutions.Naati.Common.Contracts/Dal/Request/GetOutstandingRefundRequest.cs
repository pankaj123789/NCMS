using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetOutstandingRefundRequest
    {
        public IEnumerable<string> CreditNotes { get; set; }
    }
}
