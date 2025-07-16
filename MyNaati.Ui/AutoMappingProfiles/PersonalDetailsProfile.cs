using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.CredentialApplication;
using MyNaati.Ui.ViewModels.PersonalDetails;
using MyNaati.Ui.ViewModels.Shared;
using Newtonsoft.Json;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class PersonalDetailsProfile : Profile
    {
        public PersonalDetailsProfile()
        {
            CreateMap<PersonalAddress, AddressSearchResultItem>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.AddressId))
                .ForMember(x => x.IsExaminer, m => m.Ignore())
                .ForMember(x => x.IsFuturePractitioner, m => m.Ignore())
                .ForMember(x => x.IsPractitioner, m => m.Ignore())
                .ForMember(x => x.EditUrl, m => m.Ignore());

            CreateMap<PersonalAddress, AddressListResultItem>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.AddressId))
                .ForMember(x => x.StreetDetails, m => m.MapFrom(x => x.StreetDetails))
                .ForMember(x => x.Selected, m => m.Ignore());

            CreateMap<PersonalEmail, EmailSearchResultItem>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.EmailId))
                .ForMember(x => x.IsExaminer, m => m.Ignore())
                .ForMember(x => x.IsFuturePractitioner, m => m.Ignore())
                .ForMember(x => x.IsPractitioner, m => m.Ignore())
                .ForMember(x => x.EditUrl, m => m.Ignore());

            CreateMap<PersonalPhone, PhoneSearchResultItem>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.PhoneId))
                .ForMember(x => x.IsExaminer, m => m.Ignore())
                .ForMember(x => x.IsFuturePractitioner, m => m.Ignore())
                .ForMember(x => x.IsPractitioner, m => m.Ignore())
                .ForMember(x => x.EditUrl, m => m.Ignore());

            CreateMap<PersonalViewAddress, AddressEditModel>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.AddressId))
                .ForMember(x => x.OdAddressVisibilityTypeId, m => m.MapFrom(x => x.OdAddressVisibilityTypeId))
                .ForMember(x => x.OdAddressVisibilityTypeName, m => m.MapFrom(x => x.OdAddressVisibilityTypeName))
                .ForMember(x => x.SuburbName, m => m.MapFrom(a => GetSuburbNameFromId(a.PostcodeId)))
                .ForMember(x => x.CountryName, m => m.MapFrom(a => GetCountryNameFromId(a.CountryId)))
                .ForMember(x => x.Success, m => m.Ignore())
                .ForMember(x => x.Errors, m => m.Ignore());

            CreateMap<AddressEditModel, PersonalEditAddress>()
                .ForMember(x => x.AddressId, m => m.MapFrom(x => x.Id))
                .ForMember(x => x.CountryId, m => m.MapFrom(x => x.CountryId))
                .ForMember(x => x.PostcodeId, m => m.MapFrom(x => x.PostcodeId))
                .ForMember(x => x.IsFromAustralia, m => m.MapFrom(x => x.IsFromAustralia))
                .ForMember(x => x.IsPreferred, m => m.MapFrom(x => x.IsPreferred))
                .ForMember(x => x.StreetDetails, m => m.MapFrom(x => x.StreetDetails))
                .ForMember(x => x.ValidateInExternalTool, m => m.MapFrom(x => x.ValidateInExternalTool))
                .ForMember(x => x.ExaminerCorrespondence, m => m.MapFrom(x => x.ExaminerCorrespondence));

            CreateMap<PersonalViewEmail, EmailEditModel>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.EmailId))
                .ForMember(x => x.Success, m => m.Ignore())
                .ForMember(x => x.Errors, m => m.Ignore())
                .ForMember(x => x.PrimaryEmail, m => m.Ignore());

            CreateMap<EmailEditModel, PersonalEditEmail>()
                .ForMember(x => x.EmailId, m => m.MapFrom(x => x.Id))
                .ForMember(x => x.IsExaminer, m => m.Ignore());

            CreateMap<PersonalViewPhone, PhoneEditModel>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.PhoneId))
                .ForMember(x => x.Number, m => m.MapFrom(x => x.PhoneNumber))
                .ForMember(x => x.Success, m => m.Ignore())
                .ForMember(x => x.Errors, m => m.Ignore());

            CreateMap<PhoneEditModel, PersonalEditPhone>()
                .ForMember(x => x.PhoneId, m => m.MapFrom(x => x.Id))
                .ForMember(x => x.PhoneNumber, m => m.MapFrom(x => x.Number))
                .ForMember(x => x.ExaminerCorrespondence, m => m.MapFrom(x => x.ExaminerCorrespondence));

            CreateMap<PersonalDetailsModel, PersonalDetailsModel>();

            CreateMap<PersonVerificationModel, VerifyPersonRequestContract>()
                .ForMember(x => x.NaatiNumber, y => y.MapFrom(z => z.NAATINumber))
                .ForMember(x => x.DateOfBirth, y => y.MapFrom(z => JsonConvert.DeserializeObject<IList<DateTime>>($"[\"{z.DateOfBirth}\"]")[0]));

            CreateMap<AddressContract, AddressModel>()
                .ForMember(x => x.OdAddressVisibilityTypes, m => m.Ignore())
                .ForMember(x => x.IsPreferred, m => m.Ignore())
                .ForMember(x => x.Success, m => m.Ignore())
                .ForMember(x => x.ExaminerCorrespondence, m => m.Ignore())
                .ForMember(x => x.Errors, m => m.Ignore())
                .ForMember(x => x.OdAddressVisibilityTypeId, m => m.Ignore())
                .ForMember(x => x.OdAddressVisibilityTypeName, m => m.Ignore());

            CreateMap<PersonDetailsResponse, CustomerDetailsModel>()
                .ForMember(x => x.NAATINumber, y => y.MapFrom(z => z.NaatiNumber));
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