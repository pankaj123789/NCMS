
namespace Ncms.Contracts.Models.CredentialRequest
{
    public class VenueModel
    {
        public int VenueId { get; set; }
        public int TestLocationId { get; set; }
        public string Address { get; set; }
        public int Capacity { get; set; }
        public string Name { get; set; }
        public string PublicNotes { get; set; }

        public string NamePlusCapacity => $"{Name ?? string.Empty} ({Naati.Resources.Shared.Capacity} {Capacity})";
    }
}
