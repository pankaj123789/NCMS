namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class StandardTestComponentContract
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
        public int MarkingResultTypeId { get; set; }
        public int TestComponentResultId { get; set; }
    }
}