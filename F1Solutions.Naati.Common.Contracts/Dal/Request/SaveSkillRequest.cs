using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveSkillRequest
    {
        public int? SkillId { get; set; }
        public int SkillTypeId { get; set; }
        public int Language1Id { get; set; }
        public int? Language2Id { get; set; }
        public int DirectionTypeId { get; set; }
        public int UserId { get; set; }
        public IEnumerable<int> CredentialApplicationTypeId { get; set; }
        public string Note { get; set; }
    }
}