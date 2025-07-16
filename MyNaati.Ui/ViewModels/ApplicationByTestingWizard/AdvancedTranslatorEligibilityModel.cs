using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    [AdvancedTranslatorRequiresAllChecked(ErrorMessage = "You need to meet all the eligibility criteria.")]
    public class AdvancedTranslatorEligibilityModel
    {
        public AdvancedTranslatorEligibilityModel()
        {
            CriteriaList = new List<SelectListItem>();
            ProfessionalTranslatorCredentialList = new List<ProfessionalTranslatorCredentialItem>();
        }

        public bool HasDegree { get; set; }

        public bool HasReference { get; set; }

        public IList<SelectListItem> CriteriaList { get; set; }

        public List<ProfessionalTranslatorCredentialItem> ProfessionalTranslatorCredentialList { get; set; }
    }

    public class ProfessionalTranslatorCredentialItem
    {
        public bool Selected { get; set; }
        
        public int TestId { get; set; }

        public string Language { get; set; }

        public bool ToEnglish { get; set; }

        public string LanguageAndDirection 
        {
            get
            {
                string formater = ToEnglish ? "{0} to {1}" : "{1} to {0}";
                return string.Format(formater, Language, "English");
            }
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AdvancedTranslatorRequiresAllCheckedAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            var translatorEligibility = (AdvancedTranslatorEligibilityModel)value;

            if (translatorEligibility == null)
                return false;

            return (translatorEligibility.HasDegree
                    && translatorEligibility.HasReference
                    && translatorEligibility.ProfessionalTranslatorCredentialList.All(e => e.Selected));
        }
    }
}