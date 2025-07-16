using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class ApplicationFormSectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public List<ApplicationFormQuestionModel> Questions { get; set; }
    }
}