using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class AddressListViewModel
    {
        public AddressListViewModel()
        {
            AddressForCreate = new AddressEditModel();
            AddressForEdit = new AddressEditModel();
            AddressForCreateGoogle = new AddressEditGoogleModel();
            AddressForEditGoogle = new AddressEditGoogleModel();
        }

        public string LastUpdated { get; set; }

        public int NAATINumber { get; set; }
    

        //These are here so we can refer to them in calls to HtmlHelper in the View
        //This whole object will actually be posted to the Json address update method,
        //since it seems hard to wrangle EditorFor into loading from one viewmodel 
        //and saving to another. Better solutions invited!
        public AddressEditModel AddressForCreate { get; private set; }
        public AddressEditModel AddressForEdit { get; private set; }
        public AddressEditGoogleModel AddressForCreateGoogle { get; set; }
        public AddressEditGoogleModel AddressForEditGoogle { get; set; }


        public void PopulateLookups(ILookupProvider lookupProvider)
        {
        }

        public bool HasPDListing { get; set; }
    }
}