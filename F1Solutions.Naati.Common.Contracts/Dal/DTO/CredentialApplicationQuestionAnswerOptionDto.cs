using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationQuestionAnswerOptionDto
    {
        public int Id { get; set; }
        public int FormAnswerOptionId { get; set; }
        public bool DefaultAnswer { get; set; }
        public int? CredentialApplicationFieldId { get; set; }
        public int DisplayOrder { get; set; }
        public string Option { get; set; }
        public string Description { get; set; }
        public int? FieldOptionId { get; set; }
        public string  FieldData { get; set; }
        public IEnumerable<CredentialApplicationFormAnswerOptionActionTypeDto> Actions { get; set; }
        public IEnumerable<CredentialApplicationFormAnswerOptiondDocumentTypeDto> Documents { get; set; }
    }
}