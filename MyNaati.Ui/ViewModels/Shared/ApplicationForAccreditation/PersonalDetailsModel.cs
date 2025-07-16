using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.UI;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation
{
    public class PersonalDetailsModel
    {
        private const string NAATI_NUMBER_VALIDATION_MESSAGE = "If this is not your first NAATI application you must provide your Customer Number.";
        private const string DATE_OF_BIRTH_VALIDATION_MESSAGE = "Date of birth cannot be greater than the current date.";

        private readonly ILookupProvider mLookupProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public PersonalDetailsModel()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
            _autoMapperHelper = ServiceLocator.Resolve<IAutoMapperHelper>();

            var user = HttpContext.Current.User;

            FirstApplication = user.IsNonCandidate();
            NaatiNumber = user.Identity.Name;
        }

        [UIHint("booleanToRadio")]
        [DisplayName("Is this your first NAATI application?")]
        [Required()]
        public bool? FirstApplication { get; set; }

        [DisplayName("Customer Number")]
        [ValidateNaatiNumber(true)]
        [ValidateNaatiNumberWhenIsNotFirstApplicaiton] // See bug 24079
        public string NaatiNumber { get; set; }

        [DisplayName("Given name")]
        [Required()]
        public string GivenName { get; set; }

        [DisplayName("Other names")]
        public string OtherNames { get; set; }

        [DisplayName("Alternative given name")]
        public string AlternativeGivenName { get; set; }

        [DisplayName("Family name")]
        [Required()]
        public string FamilyName { get; set; }

        [DisplayName("Alternative family name")]
        public string AlternativeFamilyName { get; set; }

        [DisplayName("Date of birth")]
        [Required()]
        [ValidateDateOfBirth]
        public DateTime? DateOfBirth { get; set; }

        [UIHint("booleanToRadio")]
        [DisplayName("Gender")]
        public bool? IsGenderMale { get; set; }

        public IList<SelectListItem> TitleList { get; set; }
        public IList<SelectListItem> CountryList { get; set; }

        [DisplayName("Title")]
        [UIHint("ListToSelect")]
        public int? TitleId { get; set; }

        [DisplayName("Country of birth")]
        [UIHint("ListToSelect")]
        public int? CountryId { get; set; }

        public string Title
        {
            get { return mLookupProvider.PersonTitles.Where(e => e.SamId == TitleId).Select(e => e.DisplayText).SingleOrDefault(); }
        }

        public string Country
        {
            get
            {
                var country = mLookupProvider.Countries.Where(e => e.SamId == CountryId).Select(e => e.DisplayText).SingleOrDefault();
                return string.IsNullOrWhiteSpace(country) ? string.Empty : country;
            }
        }

        public void UpdateFrom(PersonalDetailsModel inputModel)
        {
            //preserve these so we don't need to reload them
            var existingTitles = this.TitleList;
            var existingCountries = this.CountryList;

            _autoMapperHelper.Mapper.Map(inputModel, this);

            this.TitleList = existingTitles;
            this.CountryList = existingCountries;
        }

        public void BuildLists()
        {
            TitleList = BuildTitleList();
            CountryList = BuildCountryList();
        }

        private List<SelectListItem> BuildCountryList()
        {
            return mLookupProvider.Countries.ToSelectList(t => t.DisplayText, t => t.SamId.ToString()).PrependDefaultItem().ToList();
        }

        private IList<SelectListItem> BuildTitleList()
        {
            List<SelectListItem> titleList = mLookupProvider.PersonTitles.Where(t => t.IsStandardTitle)
                .ToSelectList(t => t.DisplayText, t => t.SamId.ToString())
                .PrependDefaultItem().ToList();

            return titleList;
        }

        private class ValidateNaatiNumberWhenIsNotFirstApplicaiton : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PersonalDetailsModel)validationContext.ObjectInstance;
                if (model.FirstApplication.HasValue &&
                    !model.FirstApplication.Value &&
                    string.IsNullOrWhiteSpace(model.NaatiNumber))
                {
                    return new ValidationResult(NAATI_NUMBER_VALIDATION_MESSAGE);
                }

                return null;
            }
        }

        private class ValidateDateOfBirth : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PersonalDetailsModel)validationContext.ObjectInstance;

                if (model.DateOfBirth.HasValue && model.DateOfBirth.Value.Date > DateTime.Today)
                {
                    return new ValidationResult(DATE_OF_BIRTH_VALIDATION_MESSAGE);
                }

                return null;
            }
        }
    }
}