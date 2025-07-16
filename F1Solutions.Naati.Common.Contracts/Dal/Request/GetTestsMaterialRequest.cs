using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestsMaterialRequest
    {
        public int UserId { get; set; }
        public PanelRoleCategoryName[] RoleCategories { get; set; }
        public bool ListAllStatuses { get; set; }
        public bool PrimaryContact { get; set; }
    }
}