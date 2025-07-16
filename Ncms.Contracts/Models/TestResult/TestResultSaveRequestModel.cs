using System.Collections.Generic;

namespace Ncms.Contracts.Models.TestResult
{
    public class TestResultSaveRequestModel
    {
        public int TestResultId { get; set; }
        public Dictionary<string, string> ReprocessOptions { get; set; }
        public Dictionary<string, List<Dictionary<string, object>>> DataSet { get; set; }
    }
}
