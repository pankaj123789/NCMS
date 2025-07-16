using System;
using System.Linq;
using System.Web.Mvc;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.Credential;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class PersonalDetailsModel
    {
        public PersonalDetailsModel()
        {
            AddressModel = new AddressEditGoogleModel();
            EmailModel = new EmailEditModel();
            PhoneModel = new PhoneEditModel();
            Website = new WebsiteEditModel();
        }

        public AddressEditGoogleModel AddressModel { get; set; }
        public EmailEditModel EmailModel { get; set; }
        public PhoneEditModel PhoneModel { get; set; }
        public WebsiteEditModel Website { get; set; }
        public CredentialDetailRequestModel CredentialDetailRequest { get; set; }

        public DateTime? LastUpdated { get; set; }
        public int NaatiNumber { get; set; }
        public bool HasPdListing { get; set; }

        public bool IsPractitioner { get; set; }
        public bool IsFuturePractitioner { get; set; }
        public bool IsExaminer { get; set; }
        public bool IsFormerPractitioner { get; set; }

        public void PopulateLookups(ILookupProvider lookupProvider)
        {
            AddressModel.CountryName = lookupProvider.Countries.Single(c => c.IsHomeCountry).DisplayText;

            var sourceList = lookupProvider.OdAddressVisibilityTypes.Select(y => new SelectListItem { Text = y.DisplayText, Value = y.SamId.ToString() }).ToList();

            AddressModel.OdAddressVisibilityTypes = sourceList;
        }
    }
}
