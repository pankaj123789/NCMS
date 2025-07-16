using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class TestAssessmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [LegalCharacters('\r', '\n')]
        public string Comment { get; set; }
        public int? SelectedBand { get; set; }
        public List<TestBandModel> Bands { get; set; }
    }
}