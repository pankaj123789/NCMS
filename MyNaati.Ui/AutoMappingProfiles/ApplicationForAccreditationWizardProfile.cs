using System;
using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class ApplicationForAccreditationWizardProfile : Profile
    {
        public ApplicationForAccreditationWizardProfile()
        {
            CreateMap<PersonalViewAddress, ViewModels.Shared.ApplicationForAccreditation.AddressModel>()

                .ForMember(x => x.CountryId, m => m.MapFrom(x => x.CountryId))
                .ForMember(x => x.OdAddressVisibilityTypeId, m => m.MapFrom(x => x.OdAddressVisibilityTypeId))
                .ForMember(x => x.OdAddressVisibilityTypeName, m => m.MapFrom(x => x.OdAddressVisibilityTypeName))
                .ForMember(x => x.CountryName, m => m.MapFrom(x => GetCountryNameFromId(x.CountryId)))
                .ForMember(x => x.IsAustralia, m => m.MapFrom(x => x.IsFromAustralia))
                .ForMember(x => x.StreetDetails, m => m.MapFrom(x => x.StreetDetails))
                .ForMember(x => x.SuburbId, m => m.MapFrom(x => x.PostcodeId))
                .ForMember(x => x.SuburbName, m => m.MapFrom(x => GetSuburbNameFromId(x.PostcodeId)));

            CreateMap<PersonalDetailsExtended, ViewModels.Shared.ApplicationForAccreditation.PersonalDetailsModel>()
                .ForMember(x => x.AlternativeFamilyName, m => m.MapFrom(x => x.AlternativeFamilyName))
                .ForMember(x => x.AlternativeGivenName, m => m.MapFrom(x => x.AlternativeGivenName))
                .ForMember(x => x.CountryId, m => m.MapFrom(x => x.CountryId))
                .ForMember(x => x.CountryList, m => m.Ignore())
                .ForMember(x => x.DateOfBirth, m => m.MapFrom(x => x.DateOfBirth))
                .ForMember(x => x.FamilyName, m => m.MapFrom(x => x.FamilyName))
                .ForMember(x => x.FirstApplication, m => m.Ignore())
                .ForMember(x => x.GivenName, m => m.MapFrom(x => x.GivenName))
                .ForMember(x => x.IsGenderMale, m => m.MapFrom(x => x.IsGenderMale))
                .ForMember(x => x.NaatiNumber, m => m.MapFrom(x => x.NaatiNumber))
                .ForMember(x => x.OtherNames, m => m.MapFrom(x => x.OtherNames))
                .ForMember(x => x.TitleId, m => m.MapFrom(x => GetTitleIdFromTitle(x.Title)))
                .ForMember(x => x.TitleList, m => m.Ignore());

            CreateMap<PersonalViewPhone, ViewModels.Shared.ApplicationForAccreditation.PhoneEditModel>()
                .ForMember(x => x.AreaCode, m => m.MapFrom(x => x.AreaCode))
                .ForMember(x => x.IsPreferred, m => m.MapFrom(x => x.IsPreferred))

                .ForMember(x => x.CountryCode, m => m.MapFrom(x => x.CountryCode))
                .ForMember(x => x.Id, m => m.MapFrom(x => x.PhoneId))
                .ForMember(x => x.Number, m => m.MapFrom(x => x.PhoneNumber));

            CreateMap<PersonalViewEmail, ViewModels.Shared.ApplicationForAccreditation.EmailModel>()

                .ForMember(x => x.Email, m => m.MapFrom(x => x.Email))
                .ForMember(x => x.Id, m => m.MapFrom(x => x.EmailId))
                .ForMember(x => x.IsPreferred, m => m.MapFrom(x => x.IsPreferred));
        }

        private int? GetTitleIdFromTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                return null;

            return ServiceLocator.Resolve<ILookupProvider>().PersonTitles.Single(t => string.Equals(t.DisplayText, title, StringComparison.InvariantCultureIgnoreCase)).SamId;
        }

        private string GetSuburbNameFromId(int id)
        {
            if (id == 0)
                return string.Empty;

            return ServiceLocator.Resolve<ILookupProvider>().Postcodes.Single(p => p.SamId == id).DisplayText;
        }

        private string GetCountryNameFromId(int id)
        {
            if (id == 0)
                return string.Empty;

            return ServiceLocator.Resolve<ILookupProvider>().Countries.Single(c => c.SamId == id).DisplayText;
        }
    }
}