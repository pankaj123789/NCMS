using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class EmailListViewModel
    {
        public EmailListViewModel()
        {
            EmailForCreate = new EmailEditModel();
            EmailForEdit = new EmailEditModel();
        }

        public string LastUpdated { get; set; }

       

        public EmailEditModel EmailForCreate { get; set; }
        public EmailEditModel EmailForEdit { get; set; }

        public void PopulateLookups(ILookupProvider lookupProvider)
        {
        }
    }
}