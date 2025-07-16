using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MembershipDto
    {
        public int PanelMembershipId { get; set; }
        public int PersonId { get; set; }
        public int NAATINumber { get; set; }
        public string Name { get; set; }
        public int PanelId { get; set; }
        public string PanelName { get; set; }
        public int PanelRoleId { get; set; }
        public string PanelRole { get; set; }
        public bool IsExaminerRole { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool HasUnavailability { get; set; }
        public bool HasMarkingRequests { get; set; }
        public bool HasMaterialRequests { get; set; }
        public IEnumerable<int> CredentialTypeIds { get; set; }
        public IEnumerable<int> MaterialCredentialTypeIds { get; set; }
        public IEnumerable<int> CoordinatorCredentialTypeIds { get; set; }
        public PanelRoleCategoryName PanelRoleCategory { get; set; }
        public IList<ExaminerUnavailableContract> UnAvailableExaminers { get; set; }
        public int InProgress { get; set; }
        public int Overdue { get; set; }
    }
}