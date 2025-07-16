using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{
    public class RefundRequestsGroupingModel
    {
        public IList<RefundApprovalModel> Items { get; set; }

        public RefundRequestsGroupingModel()
        {
            Items = new List<RefundApprovalModel>();
        }
    }
}
