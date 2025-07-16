using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateTestSessionRequest
    {
        public int TestSessionId { get; set; }
        public int VenueId { get; set; }
        public string Name { get; set; }
        public DateTime TestDateTime { get; set; }
        public int? ArrivalTime { get; set; }
        public int? Duration { get; set; }
        public bool IsActive { get; set; }
    }
}