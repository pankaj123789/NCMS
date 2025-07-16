using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationFormSectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }
        public IEnumerable<CredentialApplicationFormQuestionDto> Questions { get; set; }
    }
}