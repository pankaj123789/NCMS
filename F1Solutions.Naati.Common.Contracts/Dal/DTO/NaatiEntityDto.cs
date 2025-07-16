namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class NaatiEntityDto
    {
        public int EntityId { get; set; }
        public int? PersonId { get; set; }
        public int? InstitutionId { get; set; }
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public string PrimaryEmail { get; set; }
    }
}
