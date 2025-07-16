using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class TestComponentModel
    {
        public int Id { get; set; }
        public int TotalMarks { get; set; }
        [Required]
        [RegularExpression(@"^\d{0,2}(\.\d{1,2})?$", ErrorMessage = "Invalid value for {0}.")]
        public double? Mark { get; set; }
        public double PassMark { get; set; }
        public int ComponentNumber { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string TypeName { get; set; }
        public string TypeLabel { get; set; }
        public int GroupNumber { get; set; }
        public bool WasAttempted { get; set; }
        public bool ReadOnly { get; set; }
        public int MinExaminerCommentLength { get; set; }
        public int MaxCommentLength { get; set; }
        public List<TestCompetenceModel> Competencies { get; set; }
    }
}