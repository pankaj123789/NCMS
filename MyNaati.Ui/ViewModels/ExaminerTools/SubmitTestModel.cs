using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class SubmitTestModel
    {
        public string Action { get; set; }
        public List<TestModel> TestList { get; set; }
        public int? TestResultID { get; set; }
        public List<TestComponentModel> Components { get; set; }
        [Required]
        [Display(Name = "Mark comments")]
        [LegalCharacters('\r', '\n', '\t')]
        public string Comments { get; set; }
        [LegalCharacters('\r', '\n', '\t')]
        public string Feedback { get; set; }
        public List<string> ReasonsForPoorPerformance { get; set; }
        public IEnumerable<ReasonForFailureModel> PrimaryReasonsForFailure { get; set; }
        public IEnumerable<DocumentTypeModel> DocumentTypes { get; set; }
        public int? PrimaryReasonForFailure { get; set; }
        public List<KeyValuePair<int, string>> Attachments { get; set; }
        public float OverallPassMark { get; set; }
        public string LanguageName { get; set; }
        public TestType TestType { get; set; }
        public string ReturnElement { get; set; }
    }

    public enum TestType
    {
        Standard = 1,
        Rubric = 2
    }

    public class ReasonForFailureModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class DocumentTypeModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }
}