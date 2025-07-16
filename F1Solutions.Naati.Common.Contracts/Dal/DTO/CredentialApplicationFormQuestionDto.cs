using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationFormQuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public int AnswerTypeId { get; set; }
        public string AnswerTypeName { get; set; }
        public int? CredentialApplicationFieldId { get; set; }
        public IEnumerable<object> Responses { get; set; }
        public IEnumerable<CredentialApplicationQuestionAnswerOptionDto> AnswerOptions { get; set; }
        public IEnumerable<CredentialApplicationFormQuestionLogicDto> QuestionLogics { get; set; }

    }
}