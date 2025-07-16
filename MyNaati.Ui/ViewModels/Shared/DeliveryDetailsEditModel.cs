using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class DeliveryDetailsEditModel
    {
        private const int AUSTRALIA_COUNTRY_ID = 13;

        public DeliveryDetailsEditModel()
        {
            TemporaryAddress = new AddressDetailsModel();
            SelectedAddress = new AddressDetailsModel();
        }

        public AddressDetailsModel TemporaryAddress { get; private set; }
        public AddressDetailsModel SelectedAddress { get; private set; }

        public Guid WizardId { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "A name is required.")]
        public string Name { get; set; }

        [AddressSelectedValidator]
        public bool AnyAddressSelected { get; set; }

        public int SelectedAddressId { get; set; }        

        public class AddressSelectedValidator : ValidationAttribute
        {
            private const string mErrorMessage = "You must selected an address.";

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (DeliveryDetailsEditModel)validationContext.ObjectInstance;

                if (!model.AnyAddressSelected)
                {
                    return new ValidationResult(mErrorMessage, new[] { "Selected" });
                }

                return null;
            }
        }        

        public void CopyFieldsFromAddAddressModel(AddAddressModel addAddressModel)
        {
            TemporaryAddress.Address = addAddressModel.Address;
            TemporaryAddress.CountryId = addAddressModel.IsFromAustralia ? AUSTRALIA_COUNTRY_ID : addAddressModel.CountryId; //TODO: find a way to do this better.            
            TemporaryAddress.PostcodeId = addAddressModel.PostcodeId;
            this.SelectedAddressId = 0;
        }

        public void CopySelectedFieldsFromOther(DeliveryDetailsEditModel other)
        {
            this.Name = other.Name;
            this.SelectedAddressId = other.SelectedAddressId;
            this.AnyAddressSelected = other.AnyAddressSelected;
        }
    }
}
