using System.ComponentModel.DataAnnotations;
using MyNaati.Contracts.Portal;

namespace MyNaati.Ui.ViewModels.Configuration
{
    public class ConfigurationModel
    {
        private const string DELAY_RANGE_MESSAGE = "The value entered must be between 0 and 100";
        private const string DELAY_RANGE_REQUIRED = "Days to delay must be provided";

        public ConfigurationModel()
        { }

        public ConfigurationModel(IConfigurationService configurationService)
        {
            ShowVerifyCredentials = configurationService.GetShowVerifyCredentials();
            DaysToDelayAccreditation = configurationService.GetDaysToDelayAccreditation();
            ShowPhoto = configurationService.GetShowPhoto();
            PaymentRequiredForPDListing = configurationService.GetPaymentRequiredForPDListing();
        }

        public bool ShowVerifyCredentials { get; set; }

        public bool ShowPhoto { get; set; }

        public bool PaymentRequiredForPDListing { get; set; }

        public bool ChangesSaved { get; set; }

        [UIHint("integer")]
        [Required(ErrorMessage = DELAY_RANGE_REQUIRED)]
        [Range(0, 100, ErrorMessage = DELAY_RANGE_MESSAGE)]
        public int DaysToDelayAccreditation { get; set; }
    }
}