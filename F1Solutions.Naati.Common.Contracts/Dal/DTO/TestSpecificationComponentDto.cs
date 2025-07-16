namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSpecificationComponentDto
    {        
        public int Id { get; set; }        
        public string TaskType { get; set; }        
        public int ComponentNumber { get; set; }        
        public string Label { get; set; }                
        public string TaskTypeDescription { get; set; }        
        public string BasedOn { get; set; }        
        public double? PassMark { get; set; }        
        public string Name { get; set; }        
        public int? TotalMarks { get; set; }        
        public int MinNaatiCommentLength { get; set; }        
        public int MinExaminerCommentLength { get; set; }
        public bool RolePlayersRequired { get; set; }
    }
}