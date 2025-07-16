using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation
{
    public class ResidencyStatusModel
    {
        private ILookupProvider mLookupProvider;

        public ResidencyStatusModel()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

        [Required]
        [DisplayName("Are you currently residing in Australia?")]
        public bool? IsCurrentlyResidingInAustralia{ get; set; }

        [DisplayName("Are you an Australian Permanent Resident or citizen?")]
        [RequiredIfResidingInAustralia]
        public bool? IsAustralianResident{ get; set; }

        [DisplayName("My country of residence is")]
        [RequiredIfNotResidingInAustralia]
        [UIHint("listToSelect")]
        public int? CountryId { get; set; }

        public string Country { get { return mLookupProvider.Countries.Where(e => e.SamId == CountryId).Select(e => e.DisplayText).SingleOrDefault(); } }

        public IList<SelectListItem> CountryList { get; set; }

        public class RequiredIfResidingInAustraliaAttribute : RequiredAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (ResidencyStatusModel)validationContext.ObjectInstance;
                if (model.IsCurrentlyResidingInAustralia != true)
                    return null;

                return base.IsValid(value, validationContext);
            }
        }

        public class RequiredIfNotResidingInAustraliaAttribute : RequiredAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (ResidencyStatusModel)validationContext.ObjectInstance;
                if (model.IsCurrentlyResidingInAustralia != false)
                    return null;

                return base.IsValid(value, validationContext);
            }
        }

        public void BuildLists()
        {
            this.CountryList = BuildCountryList();
        }

        private IList<SelectListItem> BuildCountryList()
        {
            return mLookupProvider.Countries.Where((l => !l.IsHomeCountry))
                .ToSelectList(c => c.DisplayText, c => c.SamId.ToString())
                .PrependDefaultItem();
        }
    }
}