using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreatePaymentRequest
    {
        public int UserId { get; set; }
        public IEnumerable<CreatePaymentModel> Payments { get; set; }
        public bool BatchProcess { get; set; }
    }
}