using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;


namespace MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation
{
    public class AddressModel
    {
        private readonly ILookupProvider mLookupProvider;

        public AddressModel()
        {
            IsAustralia = true;

            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

     

        [DisplayName("Location")]
        [Required]
        public bool IsAustralia { get; set; }

        [DisplayName("Street details")]
        [UIHint("stringToTextarea")]
        [Required]
        [StringLength(500, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string StreetDetails { get; set; }

        [DisplayName("Country")]
        [ValidateCountryId]
        public string CountryName { get; set; }

        public int? CountryId { get; set; }

        [DisplayName("Suburb")]
        [ValidateSuburbId]
        public string SuburbName { get; set; }

        public int? SuburbId { get; set; }

        public Postcode Suburb { get { return mLookupProvider.Postcodes.SingleOrDefault(e => e.SamId == SuburbId); }}
        
        public int OdAddressVisibilityTypeId { get; set; }
        public string OdAddressVisibilityTypeName { get; set; }

        private class ValidateCountryId : ValidationAttribute
        {
            public ValidateCountryId() : base("The {0} field is required") { }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (AddressModel)validationContext.ObjectInstance;
                if (model.IsAustralia || model.CountryId != null)
                    return null;
                else
                    return new ValidationResult(this.ErrorMessage);
            }
        }

        private class ValidateSuburbId : ValidationAttribute
        {
            public ValidateSuburbId() : base("The {0} field is required") { }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (AddressModel)validationContext.ObjectInstance;
                if (model.IsAustralia == false || model.SuburbId != null)
                    return null;
                else
                    return new ValidationResult(this.ErrorMessage);
            }
        }

        public void Normalise()
        {
            if (IsAustralia)
            {
                CountryId = null;
                CountryName = "";
            }
            else
            {
                SuburbId = null;
                SuburbName = "";
            }
        }
    }
}