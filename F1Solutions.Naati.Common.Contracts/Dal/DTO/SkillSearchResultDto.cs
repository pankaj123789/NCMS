using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class SkillSearchResultDto
    {
        public int SkillId { get; set; }
        public string DisplayName { get; set; }
        public string TypeDisplayName { get; set; }
        public int SkillTypeId { get; set; }
        public int Language1Id { get; set; }
        public int? Language2Id { get; set; }
        public int DirectionTypeId { get; set; }
        public int NumberOfExistingCredentials { get; set; }
        public int NumberOfCredentialRequests { get; set; }
        public IEnumerable<LookupTypeDto> ApplicationTypes { get; set; }
        public string Note { get; set; }
    }
}