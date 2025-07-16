using System;
using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class CredentialApplicationModel
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime StatusModified { get; set; }
        public string Status { get; set; }
        public IEnumerable<ApplicationFormSectionModel> Sections { get; set; }
    }
}