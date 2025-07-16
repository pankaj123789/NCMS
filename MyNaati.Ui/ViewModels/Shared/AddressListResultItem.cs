namespace MyNaati.Ui.ViewModels.Shared
{
    public class AddressListResultItem
    {
        public AddressListResultItem()
        {
        }

        public int Id { get; set; }
        public bool Selected { get; set; }
        public string StreetDetails { get; set; }
        public string Suburb { get;  set; }
        public string Country { get;  set; }
        public bool IsPreferred { get;  set; }
        public bool IsFromAustralia { get;  set; }        
    }
}