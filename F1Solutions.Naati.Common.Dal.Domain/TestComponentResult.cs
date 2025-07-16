namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestComponentResult : EntityBase
    {

        public virtual TestResult TestResult { get; set; }
        public virtual double Mark { get; set; }
        public virtual TestComponentType Type { get; set; }
        public virtual int? TotalMarks { get; set; }
        public virtual double? PassMark { get; set; }
        public virtual int? ComponentNumber { get; set; }
        public virtual int? GroupNumber { get; set; }
        public virtual MarkingResultType MarkingResultType { get; set; }
  
    }
}
