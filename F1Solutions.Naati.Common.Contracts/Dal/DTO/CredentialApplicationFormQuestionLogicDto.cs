namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationFormQuestionLogicDto
    {
        public int Id { get; set; }
        public int? QuestionAnswerOptionId { get; set; }
        public int? CredentialTypeId { get; set; }
        public int? CredentialRequestPathTypeId { get; set; }
        public bool Not { get; set; }
        public bool And { get; set; }
        public int Order { get; set; }
        public int Group { get; set; }
        public bool? PdPointsMet { get; set; }
        public bool? WorkPracticeMet { get; set; }

        public int? SkillId { get; set; }
    }
}