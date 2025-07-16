using Newtonsoft.Json;

namespace Ncms.Contracts.Models.Application
{
    public class RefundApprovalModel : RefundModel
    {
        public string InvoiceNumber { get; set; }
        public bool Approved { get; set; }
        public int NAATINumber { get; set; }
        public string Policy { get; set; }
    }
}
