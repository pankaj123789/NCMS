using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetActivitiesRequest
    {
        public int NaatiNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}