using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RubricMarkingCompetencyDto
    {
        public int CompetencyId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public int DisplayOrder { get; set; }
        public IEnumerable<RubricMarkingAssessmentCriterionDto> RubricMarkingAssessmentCriteria { get; set; }
    }
}