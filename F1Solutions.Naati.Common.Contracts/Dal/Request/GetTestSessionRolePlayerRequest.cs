using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestSessionRolePlayerRequest
    {
        public int TestSessionId { get; set; }
        public int TestSpecificationId { get; set; }
        public int SkillId { get; set; }
        public int LanguageId { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public bool IncludeRejected { get; set; }
        public IEnumerable<RolePlayerSorting> Sorting { get; set; }
    }
}