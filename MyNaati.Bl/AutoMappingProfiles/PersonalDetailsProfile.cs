using System;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class PersonalDetailsProfile : Profile
    {
        private string ConvertNewlinesToWeb(string databaseString)
        {
            return databaseString.Replace(Environment.NewLine, "\n");
        }

       public PersonalDetailsProfile()
       {
            CreateMap<Address, PersonalAddress>()
                .ForMember(x => x.AddressId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.IsPreferred, y => y.MapFrom(x => x.PrimaryContact))
                .ForMember(x => x.StreetDetails, y => y.MapFrom(x => ConvertNewlinesToWeb(x.StreetDetails)))
                .ForMember(x => x.Suburb, y => y.MapFrom(x => FormatSuburb(x.Postcode)))
                .ForMember(x => x.State, y => y.MapFrom(z => StateFromPostcode(z.Postcode)))
                .ForMember(x => x.Country, y => y.MapFrom(z => CountryFromCountry(z.Country)))
                .ForMember(x => x.OdAddressVisibilityTypeId, y => y.MapFrom(x => x.OdAddressVisibilityType.Id))
                .ForMember(x => x.OdAddressVisibilityTypeName, y => y.MapFrom(x => x.OdAddressVisibilityType.DisplayName))
                .ForMember(x => x.IsFromAustralia, y => y.MapFrom(x => x.Country != null && x.Country.Name == "Australia"))
                .ForMember(x => x.AddressType, y => y.Ignore());//is a string. could be "Personal Address" or something?

            CreateMap<Email, PersonalEmail>()
                .ForMember(x => x.EmailId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Email, y => y.MapFrom(x => x.EmailAddress))
                .ForMember(x => x.IsPreferred, y => y.MapFrom(x => x.IsPreferredEmail))
                .ForMember(x => x.IsCurrentlyListed, y => y.MapFrom(x => x.IncludeInPD))
                .ForMember(x => x.ContactType, y => y.Ignore())//is a string. could be "Personal Email" or something?
                .ForMember(x => x.IsLastContactInPD, y => y.Ignore());

            CreateMap<Phone, PersonalPhone>()
                .ForMember(x => x.PhoneId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.IsPreferred, y => y.MapFrom(x => x.PrimaryContact))
                .ForMember(x => x.PhoneNumber, y => y.MapFrom(x => x.Number))
                .ForMember(x => x.IsCurrentlyListed, y => y.MapFrom(x => x.IncludeInPD))
                .ForMember(x => x.IsLastContactInPD, y => y.Ignore());

            CreateMap<Address, PersonalViewAddress>()
                .ForMember(x => x.AddressId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.CountryId, y => y.MapFrom(x => x.Country.Id))
                .ForMember(x => x.IsPreferred, y => y.MapFrom(x => x.PrimaryContact))
                .ForMember(x => x.StreetDetails, y => y.MapFrom(x => $"{ConvertNewlinesToWeb(x.StreetDetails)} {x.SuburbOrCountry}"))
                .ForMember(x => x.PostcodeId, y => y.MapFrom(x => x.Postcode.Id))
                .ForMember(x => x.OdAddressVisibilityTypeId, y => y.MapFrom(x => x.OdAddressVisibilityType.Id))
                .ForMember(x => x.OdAddressVisibilityTypeName, y => y.MapFrom(x => x.OdAddressVisibilityType.DisplayName))
                .ForMember(x => x.IsFromAustralia, y => y.MapFrom(x => x.Country.Name == "Australia"));

            CreateMap<Email, PersonalViewEmail>()
                .ForMember(x => x.EmailId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.Email, y => y.MapFrom(x => x.EmailAddress))
                .ForMember(x => x.IsPreferred, y => y.MapFrom(x => x.IsPreferredEmail))
                .ForMember(x => x.IsCurrentlyListed, y => y.MapFrom(x => x.IncludeInPD));

            CreateMap<Phone, PersonalViewPhone>()
                .ForMember(x => x.PhoneId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.PhoneNumber, y => y.MapFrom(x => x.LocalNumber))
                .ForMember(x => x.IsPreferred, y => y.MapFrom(x => x.PrimaryContact))
                .ForMember(x => x.IsCurrentlyListed, y => y.MapFrom(x => x.IncludeInPD));

            CreateMap<Person, PersonalEditPerson>()
                .ForMember(x => x.NaatiNumber, y => y.MapFrom(x => x.Entity.NaatiNumber))
                .ForMember(x => x.IsEportalActive, y => y.NullSubstitute(false))
                .ForMember(x => x.EntityTypeId, y => y.MapFrom(x => x.Entity.EntityTypeId))
                .ForMember(x => x.Email, y => y.MapFrom(x => x.PrimaryAddress))
                .ForMember(x => x.Country, y => y.MapFrom(x => x.BirthCountry.Name));

            CreateMap<Person, PersonalDetailsExtended>()
                .ForMember(x => x.NaatiNumber, y => y.MapFrom(x => x.Entity.NaatiNumber.ToString()))
                .ForMember(x => x.AlternativeFamilyName, y => y.MapFrom(z => string.Empty))
                .ForMember(x => x.AlternativeGivenName, y => y.MapFrom(z => string.Empty))
                .ForMember(x => x.CountryId, y => y.MapFrom(x => GetBirthCountryId(x.BirthCountry)))
                .ForMember(x => x.DateOfBirth, y => y.MapFrom(x => x.BirthDate))
                .ForMember(x => x.FamilyName, y => y.MapFrom(x => x.Surname))
                .ForMember(x => x.OtherNames, y => y.MapFrom(z => string.Empty))
                .ForMember(x => x.IsGenderMale, y => y.MapFrom(x => SetIsGenderMale(x.Gender)))
                .ForMember(x => x.EntityTypeId, y => y.MapFrom(x => x.Entity.EntityTypeId));

            CreateMap<UpdatePhotoRequestContract, UpdatePhotoDto>();

            CreateMap<GeoLocationModel, GeoLocationDto>();

            CreateMap<GeoGeometryModel, GeoGeometryDto>();

            CreateMap<GeoComponentModel, GeoComponentDto>();

            CreateMap<GeoResultModel, GeoResultDto>();

            CreateMap<ParsedAddressDto, ParseAddressResponse>();
        }

        private int? GetBirthCountryId(Country country)
        {
            if (country == null)
                return null;
            return country.Id;
        }

        private string FormatSuburb(Postcode postcode)
        {
            var formattedString = "";

            if (postcode != null)
            {
                formattedString = string.Format("{0} {1} {2}",
                    postcode.Suburb.Name,
                    postcode.Suburb.State.Abbreviation,
                    postcode.PostCode);
            }

            return formattedString;
        }

        private bool? SetIsGenderMale(string gender)
        {
            bool? genderMale = null;

            if (gender != "X")
            {
                genderMale = gender == "M";
            }

            return genderMale;
        }

        private string StateFromPostcode(Postcode postcode)
        {
            return postcode != null ? postcode.Suburb.State.Abbreviation : string.Empty;
        }
        private string CountryFromCountry(Country country)
        {
            return country != null ? country.Name : string.Empty;
        }
    }
}
