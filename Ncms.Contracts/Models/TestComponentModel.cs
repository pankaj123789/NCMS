namespace Ncms.Contracts.Models
{
    public class TestComponentModel : ITestComponentModel
    {
        public int Id { get; set; }
        public int TotalMarks { get; set; }
        public double PassMark { get; set; }
        public double? Mark { get; set; }
        public int ComponentNumber { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string TypeName { get; set; }
        public string TypeLabel { get; set; }
        public int GroupNumber { get; set; }
        public int TypeId { get; set; }
         
        public int? MarkingResultTypeId { get; set; }

        public int TestComponentResultId { get; set; }
        public bool ReadOnly { get; set; }
    }


    public interface ITestComponentModel
    {
        int Id { get; set; }
        int? MarkingResultTypeId { get; set; }
        bool ReadOnly { get; set; }
    }


}
