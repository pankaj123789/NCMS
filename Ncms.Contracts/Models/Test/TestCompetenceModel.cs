using System.Collections.Generic;

namespace Ncms.Contracts.Models.Test
{
    public class TestCompetenceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TestAssessmentModel> Assessments { get; set; }
    }
}