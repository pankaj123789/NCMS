using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class AddAddressModel
    {
        private ILookupProvider mLookupProvider;

        public AddAddressModel()
        {
            IsFromAustralia = true;
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

        public Guid WizardId { get; set; }

        [UIHint("StringToTextArea"), DisplayName("Address")]
        [Required(ErrorMessage = "An address is required.")]
        public string Address { get; set; }

        [UIHint("booleanToRadio"), DisplayName("Location")]
        public bool IsFromAustralia { get; set; }

        [DisplayName("Suburb")]
        [PostcodeValidator]
        public int? PostcodeId { get; set; }

        [DisplayName("Country")]
        [CountryValidator]
        public int? CountryId { get; set; }

        public Postcode Suburb { get { return mLookupProvider.Postcodes.SingleOrDefault(e => e.SamId == PostcodeId); } }

        public string Postcode
        {
            get
            {
                return mLookupProvider.Postcodes
                    .Where(p => p.SamId == PostcodeId)
                    .Select(p => p.DisplayText)
                    .SingleOrDefault();
            }
        }

        public string CountryIfNotAustralia
        {
            get
            {
                return mLookupProvider.Countries
                    .Where(c => c.SamId == CountryId
                                        && !c.IsHomeCountry)
                    .Select(c => c.DisplayText)
                    .SingleOrDefault();
            }
        }

        public int? CountryIdIgnoringAustralia
        {
            get
            {
                int id = mLookupProvider.Countries
                    .Where(c => c.SamId == CountryId
                                && !c.IsHomeCountry)
                    .Select(c => c.SamId).SingleOrDefault();

                if (id == 0)
                    return null;

                return id;
            }
        }

        public string Country
        {
            get
            {
                if (IsFromAustralia)
                    return "Australia";

                return CountryIfNotAustralia;
            }
        }

        public class PostcodeValidator : ValidationAttribute
        {
            private const string mErrorMessage = "A suburb is required.";

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (AddAddressModel)validationContext.ObjectInstance;

                if (model.IsFromAustralia && model.PostcodeId == null)
                {
                    return new ValidationResult(mErrorMessage, new[] { "PostcodeId" });
                }

                return null;
            }
        }

        public class CountryValidator : ValidationAttribute
        {
            private const string mErrorMessage = "A country is required.";

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (AddAddressModel)validationContext.ObjectInstance;

                if (!model.IsFromAustralia && model.CountryId == null)
                {
                    return new ValidationResult(mErrorMessage, new[] { "CountryId" });
                }

                return null;
            }
        }

        public void CopyFieldsFromOther(AddAddressModel other)
        {
            this.Address = other.Address;
            this.CountryId = other.CountryId;
            this.IsFromAustralia = other.IsFromAustralia;            
            this.PostcodeId = other.PostcodeId;

            if (this.IsFromAustralia)
                this.CountryId = mLookupProvider.SystemValues.AustraliaCountryId;
        }
    }
}