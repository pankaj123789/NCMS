using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveUnavailabilityRequest
    {
        public int? Id { get; set; }
        public int NAATINumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}