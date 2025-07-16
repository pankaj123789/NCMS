namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ExaminerTestComponentResultDto
    {
        public int ExaminerTestComponentResultId { get; set; }
        public int ExaminerMarkingId { get; set; }
        public double Mark { get; set; }
        public int? TestComponentTypeId { get; set; }
        public int? TotalMarks { get; set; }
        public double? PassMark { get; set; }
        public int? ComponentNumber { get; set; }
        public int? GroupNumber { get; set; }
    }
}