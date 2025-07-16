using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class AvailableTestSessionDto
    {
        public int TestSessionId { get; set; }
        public string Name { get; set; }
        public DateTime TestDateTime { get; set; }
        public bool Selected { get; set; }
        public int TestSessionDuration { get; set; }
        public int TestLocationId { get; set; }
        public string TestLocation { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public int AvailableSeats { get; set; }
        public bool IsPreferedLocation { get; set; }
        public bool TestFeePaid { get; set; }
        public int BookingRejectHours { get; set; }
        public int NewCandidatesOnly { get; set; }
    }
}