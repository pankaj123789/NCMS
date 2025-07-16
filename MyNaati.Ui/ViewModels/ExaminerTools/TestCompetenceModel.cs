using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class TestCompetenceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TestAssessmentModel> Assessments { get; set; }
    }
}