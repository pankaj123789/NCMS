using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    public static class ContactMapper
    {
        public static NativeModels.Contacts ToNativeModelContacts(this PublicModels.Contacts contacts)
        {
            var result = new NativeModels.Contacts
            {
                _Contacts = new List<Contact>()
            };
            foreach (var contact in contacts._Contacts)
            {
                if (contact.HasValidationErrors)
                {
                    var baseModel = contact.ToNativeModelContact();
                    result._Contacts.Add(new NativeModels.Contact(baseModel));
                }
                else
                {
                    result._Contacts.Add(contact.ToNativeModelContact());
                }
            }
            return result;
        }
        public static NativeModels.Contact ToNativeModelContact(this PublicModels.Contact contact)
        {
            var address = contact.Addresses.Single();
            return new NativeModels.Contact()
            {
                Number = contact.AccountNumber,
                Email = contact.EmailAddress,
                DisplayName = contact.FirstName + " " + contact.LastName,
                Id = contact.ContactID,
                AddressLine1 = (address.AddressLine1.Length <100) ? address.AddressLine1:  address.AddressLine1.Substring(0,100), 
                AddressLine2 = address.AddressLine2,
                City = address.City,
                Country = address.CountryCode,
                PostalCode = address.PostalCode,
                State = address.Region
            };
        }

        public static PublicModels.Contacts ToPublicModelContacts(this NativeModels.Contacts contacts)
        {
            if(contacts._Contacts ==null)
            {
                LoggingHelper.LogWarning("Contacts not present in native model");
                return new PublicModels.Contacts();
            }
            var result = new PublicModels.Contacts()
            {
                _Contacts = new List<PublicModels.Contact>()
            };
            foreach (var contact in contacts._Contacts)
            {
                if (contact.HasValidationErrors)
                {
                    var baseModel = contact.ToPublicBaseModel();
                    result._Contacts.Add(new PublicModels.Contact(baseModel));
                }
                else
                {
                    result._Contacts.Add(contact.ToPublicModelContact());
                }
            }
            return result;
        }

        public static PublicModels.Contact ToPublicModelContact(this NativeModels.Contact contact)
        {
            return new PublicModels.Contact()
            {
                AccountNumber = contact.Number,
                EmailAddress = contact.Email,
                ContactNumber = contact.Number,
                FirstName = contact.DisplayName, //needs to be split
                LastName = contact.DisplayName, //needs to be split
                ContactID = contact.Id,
                Addresses = new List<PublicModels.Address> 
                {
                    new PublicModels.Address
                    {
                        AddressLine1 = contact.AddressLine1,
                        AddressLine2 = contact.AddressLine2,
                        City = contact.City,
                        Country = contact.Country,
                        Region = contact.State,
                        PostalCode = contact.PostalCode
                    }
                }
            };
        }

        public static bool IsUpdated(this NativeModels.Contact src, NativeModels.Contact target)
        {
            return
                src.DisplayName != target.DisplayName ||
                src.Email != target.Email ||
                src.PaymentMethodId != target.PaymentMethodId ||
                src.AddressLine1 != target.AddressLine1 ||
                src.AddressLine2 != target.AddressLine2 ||
                src.City != target.City ||
                src.State != target.State || 
                src.Country != target.Country ||
                src.PostalCode != target.PostalCode; 
        }
    }
}
