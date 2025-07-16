using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.Shared;

namespace MyNaati.Ui.ViewModels.PDListing
{
    public class WizardModel : BasePurchaseWizardModel
    {
        public WizardModel()
        {

        }

        public WizardModel(ILookupProvider lookupProvider, int orderId) : base(lookupProvider, orderId)
        {
            AddressStep = new AddressStepEditModel();
            Addresses = new List<AddressEditModel>();
            Emails = new List<ContactDetailsEditModel>();
            Phones = new List<ContactDetailsEditModel>();
            WorkAreas = new List<WorkAreasEditModel>();
            Credentials = new List<CredentialsEditModel>();

            Steps = new List<WizardStep>()
            {
                new WizardStep() { Name = "Address", Action = "Address", AllowDirectNavigation = true},
                new WizardStep() { Name = "Select Address", Action = "SelectAddress"},
                new WizardStep() { Name = "Contact Details", Action = "ContactDetails" },
                new WizardStep() { Name = "Work Areas", Action = "WorkAreas" },
                new WizardStep() { Name = "Credentials", Action = "Credentials" },
                new WizardStep() { Name = "Payment Method", Action = "PaymentMethod" },
                new WizardStep() { Name = "Payment Details", Action = "PaymentDetails" },
                new WizardStep() { Name = "Declaration", Action = "Declaration" },
                new WizardStep() { Name = "Review", Action = "Review" }
            };

            DeclarationModel = new DeclarationEditModel { UserAgrees = false };
            PaymentRequired = true;
        }

        public AddressStepEditModel AddressStep { get; set; }

        public List<AddressEditModel> Addresses { get; set; }
        public List<ContactDetailsEditModel> Emails { get; set; }
        public List<ContactDetailsEditModel> Phones { get; set; }
        public ContactDetailsEditModel Website { get; set; }
        public List<WorkAreasEditModel> WorkAreas { get; set; }
        public List<CredentialsEditModel> Credentials { get; set; }

        public bool PaymentRequired { get; set; }

        public string ExpertiseCommaSeparated
        {
            get
            {
                var listOfExpertise = new List<string>();
                foreach (var workArea in WorkAreas.Where(x => x.IsChecked))
                {
                    listOfExpertise.Add(workArea.Expertise);
                }
                return String.Join(", ", listOfExpertise);
            }
        }

        public string AddressToShow
        {
            get
            {
                string addressString = string.Empty;
                if (AddressStep.ShowFullAddress.HasValue && AddressStep.ShowFullAddress.Value == AddressType.ShowFullAddress)
                {
                    if (!Addresses.Exists(add => add.InDirectory))
                    {
                        addressString = Addresses.SingleOrDefault(a => a.IsPreferred).Address;
                    }
                    else
                    {
                        if (Addresses.Where(add => add.InDirectory).Count() > 1)
                        {
                            foreach (var address in Addresses.Where(add => add.InDirectory))
                            {
                                var street = address.StreetAddress;
                                string suburbOrCountry = string.Empty;
                                //addressString = addressString +  address.StreetAddress + Environment.NewLine;
                                if (address.Country == "Australia")
                                {
                                    suburbOrCountry = address.Suburb + " " + address.Country;
                                }
                                else
                                {
                                    suburbOrCountry = address.Suburb;
                                }
                                addressString = string.Format("{0} \n{1}", street, suburbOrCountry);
                            }
                        }
                        else
                        {
                            foreach (var address in Addresses.Where(add => add.InDirectory))
                            {
                                var street = address.StreetAddress;
                                string suburbOrCountry = string.Empty;

                                if (address.Country == "Australia")
                                {
                                    suburbOrCountry = address.Suburb + " " + address.Country;
                                }
                                else
                                {
                                    suburbOrCountry = address.Suburb;
                                }
                                addressString = string.Format("{0} \n{1}", street, suburbOrCountry);
                            }
                        }
                    }
                }
                else
                {
                    addressString = Addresses.FirstOrDefault(add => add.IsPreferred).Suburb;
                }
                return addressString;
            }
        }

        public DeclarationEditModel DeclarationModel { get; set; }
        public string Title { get; set; }

        [UIHint("dateOnly")]
        public DateTime ListingEndDate { get; set; }
        [UIHint("dateOnly")]
        public DateTime ListingStartDate { get; set; }

        public string ListingFinancialYear { get { return ListingStartDate.Year + "-" + ListingEndDate.Year; } }

        public decimal DomesticPrice { get; set; }
        public decimal OverseasPrice { get; set; }
        private bool mIsListing = true;
        private bool mIsRenewal;

        public decimal Price
        {
            get
            {
                if (DeliveryDetailsModel == null)
                    return DomesticPrice;
                return (DeliveryDetailsModel.SelectedAddress.IsFromAustralia) ? DomesticPrice : OverseasPrice;
            }
        }

        public bool IsListing
        {
            get
            {
                return mIsListing;
            }
            set
            {
                Title = value ? "List in Practitioner Directory" : "Change My Listing";
                mIsListing = value;
            }
        }

        public bool IsRenewal
        {
            get { return mIsRenewal; }
            set { mIsRenewal = value; }
        }

        public class AddressStepEditModel
        {
            public AddressType? ShowFullAddress { get; set; }
        }

        public enum AddressType
        {
            ShowFullAddress,
            DoNotShowFullAddress
        }

        public class AddressEditModel
        {
            public int Id { get; set; }
            public string Address { get; set; }
            public string Type { get; set; }
            public bool IsPreferred { get; set; }
            public string StreetAddress { get; set; }
            public string Suburb { get; set; }
            public string Country { get; set; }
            public int CountryId { get; set; }
            public string Location { get; set; }
            public bool InDirectory { get; set; }
            public int OdAddressVisibilityTypeId { get; set; }
            public string OdAddressVisibilityTypeName { get; set; }
        }

        public class AddressModel
        {
            public int Id { get; set; }
            public bool InDirectory { get; set; }
            public string Address { get; set; }
            public bool IsPreferred { get; set; }
            public string Suburb { get; set; }
            public string Country { get; set; }
        }

        public class ContactDetailsEditModel
        {
            public int Id { get; set; }
            public bool InDirectory { get; set; }
            public string Contact { get; set; }
            public string Type { get; set; }
        }

        public class WorkAreasEditModel
        {
            public int Id { get; set; }
            public string Expertise { get; set; }
            public bool IsChecked { get; set; }
        }

        public class CredentialsEditModel
        {
            public int Id { get; set; }
            public bool InDirectory { get; set; }
            public string Skill { get; set; }
            public string Level { get; set; }
            public string Direction { get; set; }
            public bool IsIndigenous { get; set; }

            [UIHint("dateOnly")]
            public DateTime? Expiry { get; set; }
        }

        public void CopyCredentialsFrom(WizardModel model)
        {
            int i = 0;
            foreach (var credential in this.Credentials)
            {
                credential.InDirectory = model.Credentials[i++].InDirectory;
            }
        }

        public List<string> GetSelectedWorkAreas()
        {
            var result = new List<string>();
            foreach (var workArea in this.WorkAreas.Where(wa => wa.IsChecked))
            {
                result.Add(workArea.Expertise);
            }

            return result;
        }

        public void UpdateContactDetails(WizardModel model)
        {
            foreach (var contact in this.Emails)
                contact.InDirectory = model.Emails.SingleOrDefault(e => e.Id == contact.Id).InDirectory;
            foreach (var contact in this.Phones)
                contact.InDirectory = model.Phones.SingleOrDefault(e => e.Id == contact.Id).InDirectory;
            if (this.Website != null)
                this.Website.InDirectory = model.Website.InDirectory;
        }

        public bool ContactDetailsValid()
        {
            return (this.Addresses.Any(add => add.IsPreferred) && (this.Phones.Count > 0 || this.Emails.Count > 0));
        }

        /// <summary>
        /// Update order price based on delivery location.
        /// </summary>
        /// <param name="isFromAustralia"></param>
        /// <param name="domesticPrice"></param>
        /// <param name="overseasPrice"></param>
        public void UpdateOrderTotalModel(bool isFromAustralia, decimal domesticPrice, decimal overseasPrice)
        {
            if (this.IsListing)
            {
                this.OrderTotalModel = new OrderTotalViewModel()
                {
                    TotalPrice = isFromAustralia ? domesticPrice : overseasPrice,
                    Items = new List<ProductOrderItem>()
                    {
                        new ProductOrderItem() {
                            AustraliaPrice = domesticPrice,
                            OverseasPrice = overseasPrice,
                            Product = IsRenewal ? ProductType.PDRenewal : ProductType.PDRegistration,
                            Quantity = 1,
                            IsAustraliaPricing = isFromAustralia,
                            Expiry = ListingEndDate
                        }
                    }
                };
            }
        }

        public bool AreCredentialSelectionsValid()
        {
            return this.Credentials.Any(c => c.InDirectory);
        }

        public bool AreContactDetailSelectionsValid()
        {
            return this.Phones.Any(c => c.InDirectory) || this.Emails.Any(e => e.InDirectory);
        }

        public bool IsAddressDetailSelectionsValid()
        {
            return this.Addresses.Any(c => c.InDirectory);
        }

        protected override void UpdateStepsForOfflineOrder()
        {
            if (Steps.Any(s => s.Action.Equals("Declaration")))
            {
                Steps.Remove(Steps.Single(s => s.Action.Equals("Declaration")));
            }

            base.UpdateStepsForOfflineOrder();
        }

        protected override void UpdateStepsForOnlineOrder()
        {
            if (!Steps.Contains(Steps.SingleOrDefault(s => s.Action.Equals("Declaration"))))
            {
                var paymentMethodIndex = Steps.IndexOf(Steps.Single(s => s.Action.Equals("PaymentMethod")));
                Steps.Insert(2 + paymentMethodIndex, new WizardStep { Action = "Declaration", AllowDirectNavigation = true, Name = "Declaration" });
            }

            base.UpdateStepsForOnlineOrder();
        }
    }
}
