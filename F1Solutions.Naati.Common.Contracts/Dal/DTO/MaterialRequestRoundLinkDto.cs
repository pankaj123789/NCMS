namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestRoundLinkDto
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public int? UserId { get; set; }
        public int? PersonId { get; set; }
        public int? PersonNaatiNumber { get; set; }
        public bool NcmsAvailable { get; set; }
    }
}
