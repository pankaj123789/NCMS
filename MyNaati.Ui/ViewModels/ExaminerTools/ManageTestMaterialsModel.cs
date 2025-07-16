using System.Collections.Generic;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class ManageTestMaterialsModel
    {
        public IEnumerable<SelectListItem> RoundStatusTypes { get; set; }
        public IEnumerable<SelectListItem> CredentialTypes { get; set; }
        public IEnumerable<SelectListItem> OverdueList { get; set; }
        public List<MaterialRequestSearchResultModel> TestMaterialsList { get; set; }
        public int TotalAvailableRows { get; set; }
        public ManageTestMaterialsModel()
        {
            OverdueList = new[]
            {
                new SelectListItem{ Text = "Select...", Value = ""},
                new SelectListItem{ Text = "Yes", Value = "Yes"},
                new SelectListItem{ Text = "No", Value = "No"},
            };
        }
    }
}
