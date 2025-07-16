using AutoMapper;
using F1Solutions.Naati.Common.Dal.Domain;
using MyNaati.Contracts.BackOffice.PractitionerDirectory;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class PDListingProfile : Profile
    {
        public PDListingProfile()
        {
            CreateMap<Address, AddressListing>()
                .ForMember(x => x.AddressId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.OdAddressVisibilityTypeId, y => y.MapFrom(x => x.OdAddressVisibilityType.Id))
                .ForMember(x => x.OdAddressVisibilityTypeName, y => y.MapFrom(x => x.OdAddressVisibilityType.DisplayName))
                .ForMember(x => x.IsPreferred, y => y.MapFrom(x => x.PrimaryContact))
                .ForMember(x => x.FullAddress, y => y.MapFrom(x => FormatFullAddress(x)))
                .ForMember(x => x.StreetAddress, y => y.MapFrom(x => x.StreetDetails.Replace("\n", ", ")))
                .ForMember(x => x.Country, y => y.MapFrom(x => x.Country.Name))
                .ForMember(x => x.CountryId, y => y.MapFrom(x => x.Country.Id))
                .ForMember(x => x.Suburb, y => y.MapFrom(x => FormatSuburb(x)))
                .ForMember(x => x.Location, y => y.MapFrom(x => FormatLocation(x)))
                .ForMember(x => x.Type, y => y.Ignore())
                .ForMember(x => x.InDirectory, y => y.Ignore());

            CreateMap<Email, ContactDetailListing>()
                .ForMember(x => x.ContactDetailsId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.IncludeInPractitionerDirectory, y => y.MapFrom(x => x.IncludeInPD))
                .ForMember(x => x.Contact, y => y.MapFrom(x => x.EmailAddress))
                .ForMember(x => x.Type, y => y.Ignore());//this could be set to 'email' or something. There are no current usages of the set

            CreateMap<Phone, ContactDetailListing>()
                .ForMember(x => x.ContactDetailsId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.IncludeInPractitionerDirectory, y => y.MapFrom(x => x.IncludeInPD))
                .ForMember(x => x.Contact, y => y.MapFrom(x => x.Number))
                .ForMember(x => x.Type, y => y.Ignore());//this could be set to 'phone' or something. There are no current usages of the set
        }

        private string FormatFullAddress(Address address)
        {
            var returnAddress = string.Format("{0}, {1}", address.StreetDetails.Replace("\n", ", "),
                address.Country.Name == "Australia"
                    ? (address.Postcode != null
                        ? address.Postcode.Suburb.Name + " " +
                          address.Postcode.Suburb.State.Abbreviation + " " +
                          address.Postcode.PostCode
                        : "")
                    : address.Country.Name);

            return returnAddress;
        }

        private string FormatSuburb(Address address)
        {
            return address.Country.Name.Equals("Australia")
                ? (address.Postcode != null
                    ? address.Postcode.Suburb.Name + " " +
                      address.Postcode.Suburb.State.Abbreviation + " " +
                      address.Postcode.PostCode
                    : "")
                : "";
        }

        private string FormatLocation(Address address)
        {
            return address.Country.Name.Equals("Australia") 
                ? (address.Postcode != null 
                    ? address.Postcode.Suburb.Name + " " + 
                      address.Postcode.Suburb.State.Abbreviation + " " + 
                      address.Postcode.PostCode 
                    : "") 
                : address.Country.Name;
        }
    }
}
