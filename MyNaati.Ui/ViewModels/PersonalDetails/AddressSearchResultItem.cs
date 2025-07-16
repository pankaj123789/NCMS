using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class AddressSearchResultItem
    {
        public AddressSearchResultItem()
        { }

        public void ResolveIsFromAustralia()
        {
            IsFromAustralia = Country == "Australia";
        }

        public void ResolveUrls(UrlHelper urlHelper)
        {
            EditUrl = string.Format("{0}/{1}", urlHelper.Action("AddressEdit", "PersonalDetails"), Id);
        }

        public int Id { get; private set; }
        public string StreetDetails { get; private set; }
        public string Suburb { get; private set; }
        public string Country { get; private set; }
        public bool IsFromAustralia { get; private set; }
        public bool IsPreferred { get; private set; }
        public int OdAddressVisibilityTypeId { get; set; }
        public string OdAddressVisibilityTypeName { get; set; }
        public string EditUrl { get; private set; }
        public bool ExaminerCorrespondence { get; set; }
        public bool IsPractitioner { get; set; }
        public bool IsFuturePractitioner { get; set; }
        public bool IsExaminer { get; set; }
    }
}
