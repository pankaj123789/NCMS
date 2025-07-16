using System;

namespace Ncms.Contracts.Models.Accounting
{
    public class QueuedOperationModel
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string TypeDisplayName { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedDateTime { get; set; }
        public DateTime? ProcessedDateTime { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public string StatusDescription { get; set; }
        public string Message { get; set; }
    }
}
