using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    public class ParaprofessionalEligibilityModel
    {
        public ParaprofessionalEligibilityModel()
        {
            EligibilityCriteria = new ParaprofessionalCriteria();
        }

        public ParaprofessionalCriteria EligibilityCriteria { get; set; }

        [Required(ErrorMessage = "You need to meet one of the eligibility criteria.")]
        public int? SelectedCriteriaId { get; set; }

        public string SelectedCriteria
        {
            get
            {
                var selectedCriteria = EligibilityCriteria.CriteriaList.Where(e => string.Equals(e.Value, SelectedCriteriaId.GetValueOrDefault(0).ToString())).SingleOrDefault();
                if (selectedCriteria == null)
                    return null;
                return selectedCriteria.Text;
            }
        }
    }

    public class ParaprofessionalCriteria
    {
        public ParaprofessionalCriteria()
        {
            CriteriaList = new List<SelectListItem>()
            {
                new SelectListItem(){ Text="", Value="0", Selected = true},
                new SelectListItem(){ Text="NAATI has provided written confirmation that I am eligible at the Professional level", Value="1", Selected = false},
                new SelectListItem(){ Text="I hold a NAATI Paraprofessional or Professional credential in another language", Value="2", Selected = false},
                new SelectListItem(){ Text="I hold a NAATI Language Aide or Recognition credential in ", Value="3", Selected = false},
                new SelectListItem(){ Text="I have completed the equivalent of Australian secondary school to year 10 (usually 4 years)<br />(supporting documentation required)", Value="4", Selected = false},
                new SelectListItem(){ Text="I have completed post-secondary studies at the equivalent of Certificate Level 3 or greater<br />(supporting documentation required)", Value="5", Selected = false},
                new SelectListItem(){ Text="I have work experience for more than two years as a translator or interpreter<br />(supporting documentation required)", Value="6", Selected = false}
            };
        }

        public IList<SelectListItem> CriteriaList { get; set; }
    }
}