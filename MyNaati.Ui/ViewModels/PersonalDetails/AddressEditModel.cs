using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;


namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class AddressEditModel
    {
        private readonly ILookupProvider mLookupProvider;

        public AddressEditModel()
        {
            IsFromAustralia = true;

            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

        public void ResolveIsFromAustralia(ILookupProvider lookupProvider)
        {
            IsFromAustralia = CountryId == lookupProvider.SystemValues.AustraliaCountryId;
        }

        public void NormaliseCountryAndSuburb(ILookupProvider lookupProvider)
        {
            if (IsFromAustralia)
            {
                //This is just so that if the user switches the UI to "Overseas", Australia isn't the indicated country
                CountryId = null;
                CountryName = string.Empty;
            }
            else
            {
                PostcodeId = 0;
                SuburbName = string.Empty;
            }
        }

        public int Id { get; set; }
        public int OdAddressVisibilityTypeId { get; set; }
        public string OdAddressVisibilityTypeName { get; set; }

        [UIHint("stringToTextarea"), DisplayName("Street details")]
        [Required(ErrorMessage = "A street address is required.")]
        [StringLength(500, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string StreetDetails { get; set; }
       
        public string SuburbName { get; set; }

        public Postcode Suburb { get { return mLookupProvider.Postcodes.SingleOrDefault(e => e.SamId == PostcodeId); } }

        [UIHint("listToSelect"), DisplayName("Suburb")]
        public int? PostcodeId { get; set; }

        public string CountryName { get; set; }

        [UIHint("listToSelect"), DisplayName("Country")]
        public int? CountryId { get; set; }

        [UIHint("booleanToRadio"), DisplayName("Location")]
        public bool IsFromAustralia { get; set; }

        [DisplayName("Preferred address")]
        public bool IsPreferred { get; set; }

        public bool Success { get; set; }

        public List<string> Errors { get; set; }
        public bool ExaminerCorrespondence { get; set; }
        public bool ValidateInExternalTool { get; set; }
    }
}
