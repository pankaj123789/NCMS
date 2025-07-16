using System;
using System.Collections.Generic;
using System.Text;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace MyNaati.Ui.ViewModels.PDSearch
{
    public class ContactDetailsModel
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

        

        public List<ViewContactDetail> ContactDetails { get; set; }

        public List<ViewAccreditation> Accreditations { get; set; }

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

        public IEnumerable<CredentialsDetailsDto> Credentails { get; set; }
        public string Website { get; set; }


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

    public struct ViewContactDetail
    {
        public string Type { get; set; }
        public string Contact { get; set; }
        public int SortOrder { get; set; }
    }

    public struct ViewAccreditation
    {
        public string Accreditation { get; set; }
        public string Expiration { get; set; }
        
    }
}
