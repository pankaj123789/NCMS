namespace MyNaati.Ui.ViewModels.Shared
{
    public class TemporaryAddressModel
    {
        public TemporaryAddressModel()
        {
            AddressDetails = new AddressDetailsModel();
        }

        public AddressDetailsModel AddressDetails { get; private set; }        
    }
}