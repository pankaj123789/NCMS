using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class PerformBatchOperationsRequest
    {
        public int UserId { get; set; }
        public int MaxBatchSize { get; set; }
        public DateTime MaxRequestedDate { get; set; }
    }
}