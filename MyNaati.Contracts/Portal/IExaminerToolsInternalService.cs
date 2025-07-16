using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.Portal
{
    
    public interface IExaminerToolsInternalService : IInterceptableservice
    {
        
        SaveTestResponse SaveTest(SaveTestRequest request);

        
        GetTestResponse GetTest(GetTestRequest request);
    }

    public class TestComponentContract
    {
        
        public int Id { get; set; }
        
        public int TotalMarks { get; set; }
        
        public double PassMark { get; set; }
        
        public double? Mark { get; set; }
        
        public int ComponentNumber { get; set; }
        
        public string Name { get; set; }
        
        public int GroupNumber { get; set; }
    }

    
    public class SaveTestRequest
    {
        
        public int UserId { get; set; }
        
        public int TestID { get; set; }
        
        public List<TestComponentContract> Components { get; set; }
        
        public string Comments { get; set; }
        public string Feedback { get; set; }
        
        public List<string> ReasonsForPoorPerformance { get; set; }
        
        public int? PrimaryReasonForFailure { get; set; }
    }

    
    public class SaveTestResponse
    {
    }

    
    public class GetTestRequest
    {
        
        public int TestID { get; set; }
        
        public int UserID { get; set; }
    }

    
    public class GetTestResponse
    {
        
        public int TestID { get; set; }
        
        public List<TestComponentContract> Components { get; set; }
        
        public string Comments { get; set; }
        public string Feedback { get; set; }
        public List<string> ReasonsForPoorPerformance { get; set; }
        
        public int? PrimaryReasonForFailure { get; set; }
    }
}
