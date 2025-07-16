using MyNaati.Ui.ViewModels.PersonalDetails;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class AddressModel : AddressEditGoogleModel
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool ValidateInExternalTool { get; set; }
        public string PostCodeId { get; set; }
    }
}