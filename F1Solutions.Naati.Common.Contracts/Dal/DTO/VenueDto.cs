namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class VenueDto
    {
        public int VenueId { get; set; }
        public int TestLocationId { get; set; }
        public string Address { get; set; }
        public string Coordinates { get; set; }
        public int Capacity { get; set; }
        public string Name { get; set; }
        public string PublicNotes { get; set; }
    }
}