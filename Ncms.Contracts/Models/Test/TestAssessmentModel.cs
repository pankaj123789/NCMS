using System.Collections.Generic;

namespace Ncms.Contracts.Models.Test
{
    public class TestAssessmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Comment { get; set; }
        public int? SelectedBand { get; set; }
        public IEnumerable<TestBandModel> Bands { get; set; }
        public IEnumerable<ExaminerResultModel> ExaminerResults { get; set; }
    }
}