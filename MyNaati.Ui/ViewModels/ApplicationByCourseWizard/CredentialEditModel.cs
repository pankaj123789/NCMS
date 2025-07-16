using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MyNaati.Contracts.BackOffice;

namespace MyNaati.Ui.ViewModels.ApplicationByCourseWizard
{
    public class CredentialEditModel
    {
        public CredentialEditModel()
        { }

        public IList<SelectListItem> ApprovedLanguages { get; set; }
        public List<CourseApproval> CourseApprovalList { get; set; }
        public List<CourseApproval> CourseApprovalFilteredList { get; set; }

        [Required(ErrorMessage = "You must select a credential type.")]
        public int? SelectedCourseApprovalId { get; set; }

        [Required(ErrorMessage = "You must provide the approved language.")]
        [DisplayName("Approved language")]
        public int? LanguageTypeId { get; set; }

        public CredentialDetailsRow BuildCredential()
        {
            var approvedCourse = CourseApprovalList.Single(x => x.CourseApprovalId == SelectedCourseApprovalId);

            var credentialDetails = new CredentialDetailsRow
            {
                ToEnglish = approvedCourse.ToEnglish,
                FromEnglish = approvedCourse.FromEnglish,
                IsInterpreter = approvedCourse.IsInterpreter,
                IsTranslator = approvedCourse.IsTranslator,
                Level = approvedCourse.AccreditationLevelId ?? 0,
                LanguageId = (int)LanguageTypeId
            };

            return credentialDetails;
        }
    }
}
