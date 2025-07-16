using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveVenueRequest
    {
        public int? VenueId { get; set; }
        public int TestLocationId { get; set; }
        public string Address { get; set; }
        public int Capacity { get; set; }
        public string Name { get; set; }
        public string PublicNotes { get; set; }
        public string Coordinates { get; set; }
        public bool Active { get; set; }
        public bool ModifiedByNaati { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedUser { get; set; }

        public bool Inactive => !Active;
    }
}