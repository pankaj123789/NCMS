using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class ActivityPointsConfigurationResponse
    {
        public double RequiredPointsPerYear { get; set; }
        public IEnumerable<SectionPointsConfiguration> Sections { get; set; }
    }
}