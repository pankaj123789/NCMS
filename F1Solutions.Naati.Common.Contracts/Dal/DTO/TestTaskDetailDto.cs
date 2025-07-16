namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestTaskDetailDto
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string TaskLabel { get; set; }
        public string TypeLabel { get; set; }
        public string TaskName { get; set; }
        public bool RoleplayersRequired { get; set; }
    }
}