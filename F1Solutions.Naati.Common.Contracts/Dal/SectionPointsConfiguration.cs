using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class SectionPointsConfiguration
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public double RequiredPointsPerYear { get; set; }
        public IEnumerable<CategoryConfiguration> Categories { get; set; }
    }
}