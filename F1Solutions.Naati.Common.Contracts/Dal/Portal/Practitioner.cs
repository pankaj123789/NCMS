using System;
using System.Collections.Generic;
using System.Text;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    
    public class Practitioner
    {
        
        public string Surname { get; set; }

        
        public string GivenName { get; set; }

        
        public string OtherNames { get; set; }

        
        public string Title { get; set; }

        
        public string Skills { get; set; }
      

        
        public bool Deceased { get; set; }

        
        public string Location { get; set; }

        
        public int Id { get; set; }

        
        public AddressDetail Address { get; set; }

        
        public List<ContactDetail> ContactDetails { get; set; }

        
        public bool ShowPhotoOnline { get; set; }

        
        public bool IsRevalidationScheme { get; set; }

        
        public string ExpiredCredentialSkills { get; set; }

        
        public byte[] Photo { get; set; }

        
        public string PractitionerNumber { get; set; }

        
        public int Hash { get; set; }
    }

    
    public class ValCount
    {
        
        public int Val { get; set; }

        
        public int Count { get; set; }
    }


    public class AddressDetail
    {
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string Title { get; set; }
        public string StreetDetails { get; set; }
        public string Country { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public int CountryId { get; set; }
        public int OdAddressVisibilityTypeId { get; set; }
        public int DefaultContryId { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1} {2} {3}", Title, GivenName, OtherNames, Surname); }
        }

        public string Address
        {
            get
            {
                var address = new StringBuilder();

                switch (OdAddressVisibilityTypeId)
                {
                    case (int)OdAddressVisibilityTypeName.DoNotShow:
                    case (int)OdAddressVisibilityTypeName.StateOnly:
                        address.AppendLine(CountryId != DefaultContryId ? Country : State);
                        break;
                    case (int)OdAddressVisibilityTypeName.StateAndSuburb:
                        address.AppendFormat(CountryId != DefaultContryId ? Country : $"{Suburb} {State} {Postcode}");
                        break;
                    case (int)OdAddressVisibilityTypeName.FullAddress:
                        address.AppendLine(StreetDetails);
                        address.AppendFormat(CountryId == DefaultContryId ? $"{Suburb} {State} {Postcode}" : $", {Country}");
                        break;
                }

                return address.ToString();
            }
        }


        public string AbsoluteUri(string uri)
        {
            var address = uri;

            if (!address.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                address = string.Format("http://{0}", address);
            }

            return address;
        }

    }

}
