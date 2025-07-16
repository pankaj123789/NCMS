using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    public class ProfessionalEligibilityModel
    {
        public ProfessionalEligibilityModel()
        {
            CriteriaList = new List<ProfessionalEligibilityCriteriaListItem>()
            {
                new ProfessionalEligibilityCriteriaListItem(){ Text="I hold a diploma (or higher) - in any field - from a recognised Vocational Education Training (VET) institution<br />(supporting documentation required)", Value = "1", Selected = false, VisibleInterpreter = true, VisibleTranslator = true },
                new ProfessionalEligibilityCriteriaListItem(){ Text="I hold a degree (or higher) - in any field - from a recognised higher education institution<br />(supporting documentation required)", Value = "2", Selected = false, VisibleInterpreter = true, VisibleTranslator = true },
                new ProfessionalEligibilityCriteriaListItem(){ Text="I have completed relevant subjects (i.e. in translating, interpreting or language studies) at post-secondary level<br />(supporting documentation required)", Value = "3", Selected = false, VisibleInterpreter = true, VisibleTranslator = true },
                new ProfessionalEligibilityCriteriaListItem(){ Text="I hold a NAATI Paraprofessional credential in the same language", Value = "4", Selected = false, VisibleInterpreter = true, VisibleTranslator = true },
                new ProfessionalEligibilityCriteriaListItem(){ Text="I hold a NAATI Professional credential in this language", Value = "5", Selected = false, VisibleInterpreter = true, VisibleTranslator = true },
                new ProfessionalEligibilityCriteriaListItem(){ Text="I hold a NAATI Professional credential in the same skill, but a different language", Value = "6", Selected = false, VisibleInterpreter = true, VisibleTranslator = true },
                new ProfessionalEligibilityCriteriaListItem(){ Text="I have work experience for more than two years as a translator or interpreter<br />(supporting documentation required)", Value = "7", Selected = false, VisibleInterpreter = false, VisibleTranslator = true }
            };

            RequiredCriteria = new List<ProfessionalCriteriaItem>();
        }

        public IList<ProfessionalEligibilityCriteriaListItem> CriteriaList { get; set; }

        // A list of the criteria that the user has met for each of the tests.
        [ValidateListItems(ErrorMessage = "You need to meet a criteria for each language")]
        public List<ProfessionalCriteriaItem> RequiredCriteria { get; set; }
    }

    public class ProfessionalEligibilityCriteriaListItem : SelectListItem
    {
        public bool VisibleTranslator { get; set; }

        public bool VisibleInterpreter { get; set; }
    }

    public class ProfessionalCriteriaItem
    {
        private ILookupProvider mLookupProvider;

        public ProfessionalCriteriaItem()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

        [Required(ErrorMessage = "You need to meet a criteria for each language")]
        [NotZero(ErrorMessage = "You need to meet a criteria for each language")]
        public int? SelectedValue { get; set; }

        public bool IsInterpreter { get; set; }

        public string Skill
        {
            get
            {
                if (IsInterpreter)
                    return mLookupProvider.AccreditationCategories.Single(e => e.KnownType == AccreditationCategoryKnownType.Interpreter).DisplayText;
                else
                    return mLookupProvider.AccreditationCategories.Single(e => e.KnownType == AccreditationCategoryKnownType.Translator).DisplayText;
            }
        }

        public string Language { get; set; }

        public string SelectedCriteria { get; set; }
    }
}