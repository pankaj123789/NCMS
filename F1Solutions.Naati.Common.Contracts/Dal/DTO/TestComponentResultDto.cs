namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestComponentResultDto
    {
        public int Id { get; set; }
        public int TotalMarks { get; set; }
        public double PassMark { get; set; }
        public double? Mark { get; set; }
        public int ComponentNumber { get; set; }
        public string Name { get; set; }
        public int GroupNumber { get; set; }
        public int TypeId { get; set; }
    }
}