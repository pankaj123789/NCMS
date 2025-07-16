using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.PDSearch
{
    public class WizardViewModel
    {
        public WizardViewModel()
        {
            Skill = new SkillStep();
            Direction = new DocumentDirectionStep();
            Language = new LanguageStep();
            SpecificLocation = new SpecificLocationStep();
            LocationDetails = new LocationDetailStep();
            RandomSearchSeed = new Random().Next();
        }

        //This is to enable consistent paging when doing randomisation of searches.
        public int RandomSearchSeed { get; set; }

        public SkillStep Skill { get; set; }

        public DocumentDirectionStep Direction { get; set; }

        private LanguageStep mLanguage;
        public LanguageStep Language
        {
            get { return mLanguage; }
            set
            { 
                mLanguage = value; 
                mLanguage.ParentModel = this;
            }
        }

        public SpecificLocationStep SpecificLocation { get; set; }
        public LocationDetailStep LocationDetails { get; set; }
    }

    public class SkillStep
    {
        [Required]
        [DisplayName("Practitioner type")]
        public SkillType? PractitionerType { get; set; }
    }

    public class DocumentDirectionStep
    {
        [Required]
        [DisplayName("Is your document in English")]
        public bool? DocumentInEnglish { get; set; }
    }

    public class LanguageStep
    {
        [Required]
        [DisplayName("Language")]
        [UIHint("radioButtonListInt")]
        public int? OtherLanguageId { get; set; }

        public IList<SelectListItem> LanguageList { get; set; }

        internal WizardViewModel ParentModel { get; set; }

        //Calculated Properties
        public string LanguageChoiceTitle
        {
            get
            {
                if (ParentModel.Skill.PractitionerType == null)
                    return string.Empty;

                if (ParentModel.Skill.PractitionerType.Value == SkillType.Interpreter)
                    return "What language do you need?";
                else if (ParentModel.Skill.PractitionerType.Value == SkillType.Translator && ParentModel.Direction.DocumentInEnglish != null)
                {
                    if (ParentModel.Direction.DocumentInEnglish.Value)
                        return "What language do you need your document in?";
                    else
                        return "What language is your document in?";
                }

                return string.Empty;
            }
        }
    }

    public class SpecificLocationStep
    {
        [Required]
        [DisplayName("Is a location required")]
        public bool? SpecificLocationRequired { get; set; }
    }

    public class LocationDetailStep
    {
        [UIHint("ListToSelect"), DisplayName("Country")]
        public int CountryId { get; set; }

        public int AustraliaCountryId { get; set; }

        [UIHint("ListToSelect"), DisplayName("State")]
        public int AustralianStateId { get; set; }

        [DisplayName("Suburb")]
        public string SuburbName { get; set; }

        [DisplayName("Postcode")]
        public string Postcode { get; set; }

        public IList<SelectListItem> StateList { get; set; }
        public IList<SelectListItem> CountryList { get; set; }
    }

    public enum SkillType
    {
        Translator,
        Interpreter
    }
}