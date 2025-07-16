using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class PhoneListViewModel
    {
        public string LastUpdated { get; set; }

        //These are here so we can refer to them in calls to HtmlHelper in the View
        //This whole object will actually be posted to the Json address update method,
        //since it seems hard to wrangle EditorFor into loading from one viewmodel 
        //and saving to another. Better solutions invited!
        public PhoneEditModel PhoneForCreate { get; set; }
        public PhoneEditModel PhoneForEdit { get; set; }

        public void PopulateLookups(ILookupProvider lookupProvider)
        {
        }
    }
}
