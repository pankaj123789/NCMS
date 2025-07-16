namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionSkillDto
    {
        public int Id { get; set; }
        public int TestSessionId { get; set; }
        public int SkillId { get; set; }
        public int? Capacity { get; set; }
        public string Name { get; set; }
    }
}