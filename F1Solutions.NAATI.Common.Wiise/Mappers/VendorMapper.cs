using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    internal static class VendorMapper
    {
        public static PublicModels.Contacts ToPublicModelContacts(this NativeModels.Vendors vendors)
        {
            return new PublicModels.Contacts()
            {
                _Contacts = vendors._Vendors.Select(x => x.ToPublicModelContact()).ToList()
            };
        }

        public static PublicModels.Contact ToPublicModelContact(this NativeModels.Vendor vendor)
        {
            return new PublicModels.Contact()
            {
                AccountNumber = vendor.number,
                ContactID = vendor.id,  
                EmailAddress = vendor.email,
                Addresses = new List<PublicModels.Address> { new PublicModels.Address
                    {
                        AddressLine1 = vendor.addressLine1,
                        AddressLine2 = vendor.addressLine2,
                        City = vendor.city,
                        Country = vendor.country,
                        Region = vendor.state,
                        PostalCode = vendor.postalCode
                   }
                }
            };
        }
    }
}
