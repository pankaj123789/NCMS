namespace MyNaati.Ui.ViewModels.Shared
{
    public class ReviewAddressModel
    {
        public ReviewAddressModel()
        {
            AddressDetails = new AddressDetailsModel();
        }

        public AddressDetailsModel AddressDetails { get; private set; }
    }
}