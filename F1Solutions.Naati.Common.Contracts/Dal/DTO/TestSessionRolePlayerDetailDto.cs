using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionRolePlayerDetailDto
    {
        public int TestSessionRolePlayerDetailId { get; set; }
        public int SkillId { get; set; }
        public int TestComponentId { get; set; }
        public int LanguageId { get; set; }
        public int RolePlayerRoleTypeId { get; set; }
        public ObjectStatusTypeName ObjectStatus { get; set; }
        public string RolePlayerRoleTypeName { get; set; }
        public string SkillName { get; set; }
        public string TestComponentName { get; set; }
        public string TaskLabel { get; set; }
        public string TaskTypeLabel { get; set; }
    }
}