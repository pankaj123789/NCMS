using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CategoryConfiguration
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryGroupName { get; set; }
        public int? PointsLismitTypeId { get; set; }
        public int? PointsLimit { get; set; }
        public int? CategoryGroupId { get; set; }
        public double? CategoryGroupRequiredPointsPerYear { get; set; }
        public IEnumerable<RequirementConfiguration>  Requirements { get; set; }
    }
}