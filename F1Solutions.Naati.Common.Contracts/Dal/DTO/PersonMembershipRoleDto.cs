using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PersonMembershipRoleDto
    {
        public int PersonId { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string PrimaryEmail { get; set; }

        public int PanelId { get; set; }
        public string PanelName { get; set; }
        public int PanelRoleId { get; set; }
        public string PanelRoleName { get; set; }
        public DateTime? StateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PanelTypeId { get; set; }

        public PanelRoleCategoryName PanelRoleCategory { get; set; }
    }
}