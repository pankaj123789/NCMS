using System.Collections.Generic;

namespace Ncms.Contracts.Models
{
    public class GetMarksResponseModel
    {
        public IList<TestComponentModel> Components { get; set; }
        public TestSpecificationPassMarkModel OverAllPassMark { get; set; }

        public string CommentsGeneral { get; set; }
        public int TestResultId { get; set; }
        public int TestMarkingTypeId { get; set; }
    }

    public class TestSpecificationPassMarkModel
    {
        public int OverAllPassMark { get; set; }
    }
    
}
