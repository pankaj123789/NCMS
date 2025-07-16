namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class CredentialRequestModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Skill { get; set; }
        public int CategoryId { get; set; }
        public int LevelId { get; set; }
        
        public int SkillId { get; set; }
        public int PathTypeId { get; set; }
    }
}