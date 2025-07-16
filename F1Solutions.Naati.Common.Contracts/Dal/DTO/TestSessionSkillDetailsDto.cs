namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionSkillDetailsDto
    {
        public int SkillId { get; set; }
        public string Description { get; set; }
        public int NumberOfTasksWithRequiredRolePlayers { get; set; }
        public int TasksWithoutRolePlayers { get; set; }
    }
}