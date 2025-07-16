using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class PhoneSearchResultItem
    {
        public PhoneSearchResultItem()
        { }

        public void ResolveUrls(UrlHelper urlHelper)
        {
            EditUrl = string.Format("{0}/{1}", urlHelper.Action("PhoneEdit", "PersonalDetails"), Id);
        }

        public int Id { get; private set; }
        public string PhoneNumber { get; private set; }
        public bool IsCurrentlyListed { get; private set; }
        public bool IsLastContactInPD { get; private set; }
        public bool IsPreferred { get; private set; }
        public string EditUrl { get; private set; }
        public bool ExaminerCorrespondence { get; set; }
        public bool IsPractitioner { get; set; }
        public bool IsFuturePractitioner { get; set; }
        public bool IsExaminer { get; set; }
    }
}
