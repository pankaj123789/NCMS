namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestRoundLinkModel
    {
        public int Id { get; set; }
        public string Link { get; set; }

        public int? UserId { get; set; }
        public int? PersonId { get; set; }
        public bool NcmsAvailable { get; set; }
    }
}
