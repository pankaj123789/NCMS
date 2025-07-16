using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestsRequest
    {
        public int UserId { get; set; }
        public bool AsChair { get; set; }
        public PanelRoleCategoryName[] RoleCategories { get; set; }
        public int[] NAATINumber { get; set; }
        public DateTime? DateAllocatedFrom;
        public DateTime? DateAllocatedTo { get; set; }
        public int[] PanelId { get; set; }
        public DateTime? DateDueFrom { get; set; }
        public DateTime? DateDueTo { get; set; }
        public string[] TestStatus { get; set; }
        public int[] PanelMembershipIds { get; set; }
    }
}