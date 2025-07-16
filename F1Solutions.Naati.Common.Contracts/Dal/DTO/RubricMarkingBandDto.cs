namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RubricMarkingBandDto
    {
        public int BandId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public string CriterionName { get; set; }
        public string CriterionLabel { get; set; }
    }
}