using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetQueuedOperationsRequest
    {
        public DateTime? RequestedFrom { get; set; }
        public DateTime? RequestedTo { get; set; }
        public IEnumerable<ExternalAccountingOperationStatusName> Statuses { get; set; }
    }
}