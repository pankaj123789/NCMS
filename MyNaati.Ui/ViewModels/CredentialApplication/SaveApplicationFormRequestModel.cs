using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class SaveApplicationFormRequestModel
    {
        public int ApplicationId { get; set; }

        public int ApplicationFormId { get; set; }
        public IList<ApplicationFormSectionModel> Sections { get; set; }

        public int NaatiNumber { get; set; }

        public int Token { get; set; }
    }
}