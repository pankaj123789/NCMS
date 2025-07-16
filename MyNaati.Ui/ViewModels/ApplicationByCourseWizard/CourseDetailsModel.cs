using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.Portal;


namespace MyNaati.Ui.ViewModels.ApplicationByCourseWizard
{
    public class CourseDetailsModel
    {
        //certain fields aren't all that nice-looking in the database at current (eg they end with commas or spaces) so these will be trimmed in these cases.
        private readonly char[] mPunctuation = { ' ', ',', '.' };

        private ILookupProvider mLookupProvider;

        public CourseDetailsModel()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

        public string Institution
        {
            get
            {
                return mLookupProvider.Institutions
                    .Where(p => p.SamId == InstitutionId)
                    .Select(p => p.DisplayText)
                    .SingleOrDefault();
            }
        }

        public string State 
        {
            get
            { 
                return mLookupProvider.States
                    .Where(p => p.SamId == StateId)
                    .Select(p => p.DisplayText)
                    .SingleOrDefault();
            }
        }

        public string Course
        {
            get
            { 
                return mLookupProvider.Courses
                    .Where(p => p.SamId == CourseId)
                    .Select(p => p.DisplayText)
                    .SingleOrDefault();
            }
        }

        [Required(ErrorMessage = "You must provide the campus.")]
        [DisplayName("Campus or Location at which you studied")]
        public string Campus { get; set; }

        [Required(ErrorMessage = "You must provide the year started.")]
        [DisplayName("Year started")]
        public int? YearStarted { get; set; }

        [Required(ErrorMessage = "You must provide the date completed.")]
        [DisplayName("Date completed")]
        public DateTime? DateCompleted { get; set; }

        [Required(ErrorMessage = "You must provide the education institution.")]
        [DisplayName("Education institution")]
        public int? InstitutionId { get; set; }

        [Required(ErrorMessage = "You must provide the state.")]
        [DisplayName("State")]
        public int? StateId { get; set; }

        [Required(ErrorMessage = "You must provide the approved course.")]
        [DisplayName("Approved course")]
        public int? CourseId { get; set; }

        public IList<SelectListItem> InstitutionList { get; set; }
        public IList<SelectListItem> CourseList { get; set; } 
        public IList<SelectListItem> StateList { get; set; }
        public IList<SelectListItem> YearsStarted { get; set; }
        public IList<SelectListItem> YearsCompleted { get; set; }

        public void BuildLists()
        {
            InstitutionList = BuildEducationInstitutionList();
            CourseList = BuildCourseList(InstitutionId);
            StateList = BuildAustralianStateList();
            YearsStarted = BuildYearsStartedList();
            YearsCompleted = BuildYearsCompletedList();
        }

        private IList<SelectListItem> BuildEducationInstitutionList()
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "", Value = "" } };
            var educationInstitutions = mLookupProvider.Institutions
                .Where(i => i.LatestEndDateForApprovedCourse.HasValue
                    && i.LatestEndDateForApprovedCourse.Value >= LatestAllowableCourseEnd
                    && i.HasAustralianAddress
                    && !i.HasIndigenousLanguagesOnly)
                .Select(x => new SelectListItem { Selected = false, Text = x.DisplayText.Trim(mPunctuation), Value = x.SamId.ToString() })
                .OrderBy(li => li.Text);
            return allItem.Union(educationInstitutions).ToList();
        }

        private IList<SelectListItem> BuildAustralianStateList()
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "", Value = "" } };
            var australianStates = mLookupProvider.States
                .Where(s => s.IsAustralian)
                .Select(x => new SelectListItem { Selected = false, Text = x.Abbreviation, Value = x.SamId.ToString() })
                .OrderBy(li => li.Text);
            return allItem.Union(australianStates).ToList();
        }

        private List<SelectListItem> BuildYearsCompletedList()
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "", Value = "" } };
            var yearsCompleted = Enumerable.Range((DateTime.Now.Year - 3), 4).Reverse<int>()
                .Select(year => new SelectListItem() { Text = year.ToString(), Value = year.ToString() });
            return allItem.Union(yearsCompleted).ToList();
        }

        private List<SelectListItem> BuildYearsStartedList()
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "", Value = "" } };
            var yearsStarted = Enumerable.Range((DateTime.Now.Year - 15), 16).Reverse<int>()
                .Select(year => new SelectListItem() { Text = year.ToString(), Value = year.ToString() });
            return allItem.Union(yearsStarted).ToList();
        }

        private static DateTime LatestAllowableCourseEnd
        {
            get { return new DateTime(DateTime.Now.Year - 3, 1, 1); }
        }

        private static IList<SelectListItem> BuildCourseList(int? institutionId)
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "", Value = "" } };
            if (institutionId.HasValue == false)
                return allItem.ToList();

            var lookupProvider = ServiceLocator.Resolve<ILookupProvider>();
            var courses = lookupProvider.Courses
                .Where(c => c.InstitutionId == institutionId
                            && c.LatestApprovalEndDate.HasValue
                            && c.LatestApprovalEndDate > LatestAllowableCourseEnd
                            && c.ApprovedForLanguages != null && c.ApprovedForLanguages.Count() > 0)
                .Select(c => new SelectListItem { Selected = false, Text = c.DisplayText, Value = c.SamId.ToString() })
                .OrderBy(li => li.Text);
            return allItem.Union(courses).ToList();
        }

        public static IList<SelectListItem> GetCourseList(int? institutionId)
        {
            return BuildCourseList(institutionId);
        }
    }
}