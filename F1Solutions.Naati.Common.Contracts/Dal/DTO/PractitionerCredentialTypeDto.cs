using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PractitionerCredentialTypeDto
    {
        public int CredentialTypeId { get; set; }
        public string ExternalName { get; set; }
        public int DisplayOrder { get; set; }
        public string Direction { get; set; }
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }
        public int SkillId { get; set; }
    }

    public class ApiPubicPractitionerCredentialTypeDto
    {
        public string ExternalName { get; set; }
        [JsonIgnore]
        public int SkillId { get; set; }
        [JsonIgnore]
        public int DisplayOrder { get; set; }
        public string Skill { get; set; }
        
    }
}