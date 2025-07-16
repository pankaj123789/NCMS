using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public class WiiseContactCreationOperation : WiiseOperation<WiiseCreateContactRequest, Contact>
    {
        public static Contact GetWiiseContact(WiiseCreateContactRequest request)
        {
            return new Contact
            {
                ContactNumber = request.AccountNumber,
                AccountNumber = request.AccountNumber,
                Name = String.IsNullOrEmpty(request.OrgName)
                    ? $"{request.FirstName} {request.LastName} - {request.AccountNumber}"
                    : $"{request.OrgName} - {request.AccountNumber}",
                FirstName = String.IsNullOrEmpty(request.OrgName) ? request.FirstName : request.OrgName,
                LastName = String.IsNullOrEmpty(request.OrgName) ? request.LastName : null,
                EmailAddress = request.Email,
                TaxNumber = request.Abn,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressType = Address.AddressTypeEnum.POBOX,
                        AddressLine1 = request.Street,
                        City = request.City,
                        Region = request.State,
                        PostalCode = request.Postcode,
                        Country = request.Country,
                        CountryCode = request.CountryCode
                    }
                }
            };
        }

        protected override async Task<Contact> ProtectedPerformOperation()
        {
            var contact = GetWiiseContact(Request);
            var contacts = new Contacts
            {
                _Contacts = new List<Contact> { contact }
            };

            var result = await Wiise.CreateContactsAsync(Token.Value, Token.Tenant, contacts);
            return result.Data._Contacts.FirstOrDefault();
        }
    }

    public class WiiseCreateContactRequest : WiiseOperationRequest
    {
        public string AccountNumber;
        public string OrgName;
        public string FirstName;
        public string LastName;
        public string Email;
        public string Abn;
        public string Street;
        public string City;
        public string State;
        public string Postcode;
        public string Country;
        public string CountryCode;
    }
}
