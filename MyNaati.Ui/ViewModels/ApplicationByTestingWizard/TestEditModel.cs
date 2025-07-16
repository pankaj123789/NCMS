using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    public class TestEditModel
    {
        public TestEditModel()
        {
            IsFromAustralia = true;
        }

        public void PopulateLookups(ILookupProvider lookupProvider)
        {
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

        public bool IsCurrentlyListed { get; set; }

        [UIHint("stringToTextarea"), DisplayName("Street details")]
        [Required(ErrorMessage = "A street address is required.")]
        public string StreetDetails { get; set; }

        public string SuburbName { get; set; }

        [UIHint("listToSelect"), DisplayName("Suburb")]
        public int? PostcodeId { get; set; }

        public string CountryName { get; set; }

        [UIHint("listToSelect"), DisplayName("Country")]
        public int? CountryId { get; set; }

        [UIHint("booleanToRadio"), DisplayName("Location")]
        public bool IsFromAustralia{ get; set; }

        [DisplayName("Preferred address")]
        public bool IsPreferred { get; set; }
   
    }
}
