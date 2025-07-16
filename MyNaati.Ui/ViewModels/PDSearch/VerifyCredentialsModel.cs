using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Ui.ViewModels.Configuration;
using MyNaati.Ui.ViewModels.Shared;

namespace MyNaati.Ui.ViewModels.PDSearch
{
    public class VerifyCredentialsModel
    {
        [Required]
        [DisplayName("Customer Number")]
        [ValidateNaatiNumber]
        public string NaatiNumber { get; set; }

        public Practitioner Practitioner { get; set; }

        public ConfigurationModel ConfigurationModel { get; set; } 
    }
}