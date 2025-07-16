namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool System { get; set; }
    }
}
