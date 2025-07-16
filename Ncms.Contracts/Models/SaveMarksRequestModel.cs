using System.Collections.Generic;

namespace Ncms.Contracts.Models
{
    public class SaveMarksRequestModel
    {
        public int TestResultId { get; set; }
        public IList<TestComponentModel> Components { get; set; }
    }
    public class SaveExaminerMarksRequestModel : SaveMarksRequestModel
    {
        public int JobExaminerId { get; set; }
        public bool IncludePreviousMarks { get; set; }
    }
}
