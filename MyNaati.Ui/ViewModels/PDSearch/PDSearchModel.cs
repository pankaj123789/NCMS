using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MyNaati.Ui.ViewModels.PDSearch
{
    //[MustEnterCriteria]
    public class PDSearchModel
    {
        public PDSearchModel()
        {
          
            LanguageList = new List<SelectListItem>
                               {new SelectListItem {Selected = true, Text = "None", Value = "0"},
                               new SelectListItem {Selected = false, Text = "Language1", Value = "48"}};
            AccreditationLevelList = new List<SelectListItem> { new SelectListItem { Selected = true, Text = "None", Value = "0" } };
            StateList = new List<SelectListItem>
                               {new SelectListItem {Selected = true, Text = "None", Value = "0"},
                               new SelectListItem {Selected = true, Text = "State1", Value = "1"}};
            CountryList = new List<SelectListItem> { new SelectListItem { Selected = true, Text = "None", Value = "0" } };
            ReCaptchaErrorMessage = "";
            RandomSearchSeed = new Random().Next();
            
        }

        public string PageHeading = "Online Directory - Advanced Search";

        //This is to enable consistent paging when doing randomisation of searches.
        public int RandomSearchSeed { get; set; }

        //These two properties are so the search wizard can hijack this view for its own purposes
        //They're named in terms of actions rather than IsSearchWizard so that the view can serve both
        //roles without getting too confusing.
        public bool OnlyShowResults { get; set; }
        public string UrlForSearchAgainButton { get; set; }
        public string ReCaptchaErrorMessage { get; set; }

        //This can currently only be specified by the wizard.
        public bool? ToEnglish { get; set; }
        
        [UIHint("listToSelect"), DisplayName("Between language")]
        [Range(1, int.MaxValue, ErrorMessage = "Both languages must be specified.")]
        public int FirstLanguageId { get; set; }
        [UIHint("listToSelect"), DisplayName("and language")]
        [Range(1, int.MaxValue, ErrorMessage = "Both languages must be specified.")]
        public int SecondLanguageId { get; set; }
        public IList<int> Skills { get; set; }
        [UIHint("listToSelect"), DisplayName("Level of expertise")]
        public int AccreditationLevelId { get; set; }
        [UIHint("listToSelect"), DisplayName("Available in state")]
        public int StateId { get; set; }
        [UIHint("listToSelect"), DisplayName("Country")]
        public int CountryId { get; set; }
        [DisplayName("Postcode")]
        public string Postcode { get; set; }
     
        [DisplayName("Family Name")]
        public string Surname { get; set; }

        public string SortMember { get; set; }

        public int AustraliaCountryId { get; set; }
        public int EnglishLanguageId { get; set; }

        public IList<SelectListItem> SecondLanguageList { get; set; }
        public IList<SelectListItem> LanguageList { get; set; }
        public IList<SelectListItem> AccreditationLevelList { get; set; }
        public IList<SelectListItem> StateList { get; set; }
        public IList<SelectListItem> CountryList { get; set; }

        public IList<string> SkillList { get; set; }
        public string CertificationDescriptorUrl { get; set; }
        public string ComplaintPolicyUrl { get; set; }
    }

    public class MustEnterCriteriaAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "At least one language must be selected";

        public MustEnterCriteriaAttribute()
            : base(DefaultErrorMessage)
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (PDSearchModel)validationContext.ObjectInstance;

            if (model.FirstLanguageId == 0 && model.SecondLanguageId == 0)
            {
                return new ValidationResult(ErrorMessageString);
            }

            return null;
        }
    }

    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}