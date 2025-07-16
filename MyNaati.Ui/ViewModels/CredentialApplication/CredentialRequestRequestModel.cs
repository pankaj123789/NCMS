namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class CredentialRequestRequestModel
    {
        public int ApplicationId { get; set; }
        public int CategoryId { get; set; }

        public string CredentialTypes { get; set; }

        public int LevelId { get; set; }

        public int SkillId { get; set; }

        public int QuestionId { get; set; }

        public int Token { get; set; }

        public int NAATINumber { get; set; }

        public int CredentialRequestPathTypeId { get; set; }
        public int ApplicationFormId { get; set; }
    }

    public class TestSessionRequestRequestModel{
        public int ApplicationId { get; set; }
        public int Token { get; set; }
    }
}