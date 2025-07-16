using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.ApplicationByCourseWizard
{
    public class CredentialDetailsModel
    {
        private readonly ILookupProvider mLookupProvider;

        public CredentialDetailsModel()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();

            RequestedCredentials = new List<CredentialDetailsRow>();
            EditModel = new CredentialEditModel();
        }

        [MinimumItemCount(1, ErrorMessage = "You must add at least one request.")]
        [MaximumItemCount(5, ErrorMessage = "A maximum of 5 credentials can be requested on a single application.")]
        public List<CredentialDetailsRow> RequestedCredentials { get; set; }

        public CredentialEditModel EditModel { get; set; }

        public void BuildLists(int? courseId, DateTime? courseDateCompleted)
        {
            EditModel.ApprovedLanguages = BuildApprovedLanguagesList(courseId, courseDateCompleted);
        }

        private List<SelectListItem> BuildApprovedLanguagesList(int? courseId, DateTime? courseDateCompleted)
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "", Value = "" } };
            var approvedLanguages = mLookupProvider.Courses.Where(c => c.SamId == courseId)
                .SelectMany(c => c.ApprovedForLanguages)
                .Where(afl => (afl.StartDate == null || courseDateCompleted >= afl.StartDate.Value)
                           && (afl.EndDate == null || courseDateCompleted <= afl.EndDate.Value))
                .Select(afl => afl.LanguageId);
            var languageNames = mLookupProvider.Languages.Where(l => approvedLanguages.Contains(l.SamId))
                .Select(l => new SelectListItem() { Text = l.DisplayText, Value = l.SamId.ToString() })
                .OrderBy(li => li.Text)
                .ToList();
            
            return allItem.Union(languageNames).ToList();
        }
    }
}