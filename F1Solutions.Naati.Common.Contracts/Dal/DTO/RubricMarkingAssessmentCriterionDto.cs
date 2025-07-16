using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RubricMarkingAssessmentCriterionDto
    {
        public int AssessmentCriterionId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public int DisplayOrder { get; set; }
        public IEnumerable<RubricMarkingBandDto> Bands { get; set; }
        public int? SelectedBandId { get; set; }
        public string Comments { get; set; }
        public IEnumerable<ExaminerResultDto> ExaminerResults { get; set; }
    }
}