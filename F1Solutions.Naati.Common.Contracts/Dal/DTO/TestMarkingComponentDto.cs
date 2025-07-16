using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestMarkingComponentDto
    {
        public int TestComponentId { get; set; }
        public int TestComponentTypeId { get; set; }
        public int ComponentNumber { get; set; }   
        public string Name { get; set; }
        public string Label { get; set; }        
        public string TypeName { get; set; }
        public string TypeLabel { get; set; }
        public string TypeDescription { get; set; }
        public int GroupNumber { get; set; }
        public bool? WasAttempted { get; set; }
        public bool? Successful { get; set; }
        public int TotalMarks { get; set; }
        public double? Mark { get; set; }
        public double PassMark { get; set; }
        public int? MarkingResultTypeId { get; set; }
        public bool ReadOnly { get; set; }
        public int? RubricTestComponentResultId { get; set; }
        public int MinNaatiCommentLength { get; set; }
        public int MinExaminerCommentLength { get; set; }
        public IEnumerable<RubricMarkingCompetencyDto> RubricMarkingCompentencies { get; set; }
    }
}