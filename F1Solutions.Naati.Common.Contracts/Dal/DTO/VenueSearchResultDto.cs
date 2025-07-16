namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class VenueSearchResultDto
    {
        public int VenueId { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public int? Capacity { get; set; }
        public string PublicNotes { get; set; }
        public int? TestLocationId { get; set; }
        public string Address { get; set; }
        public string Coordinates { get; set; }
        public bool Inactive { get; set; }
        public bool Active => !Inactive;
    }
}