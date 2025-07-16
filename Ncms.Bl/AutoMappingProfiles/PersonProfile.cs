using System;
using System.Collections.Generic;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models.Person;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<ContactPersonModel, ContactPersonDto>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.ContactPersonId));

            CreateMap<PersonEntityDto, PersonModel>()
                .ForMember(x => x.ExpiryDate, y => y.Ignore())
                .ForMember(x => x.ContactDetails, y => y.Ignore())
                .ForMember(x => x.ArchiveHistory, y => y.Ignore())
                .ForMember(x => x.NameHistory, y => y.Ignore())
                .ForMember(x => x.Credentials, y => y.Ignore())
                .ForMember(x => x.RolePlayLocations, y => y.Ignore())
                .ForMember(x => x.MaximumRolePlaySessions, y => y.Ignore())
                .ForMember(x => x.Rating, y => y.Ignore())
                .ForMember(x => x.Senior, y => y.Ignore())
                .ForMember(x => x.ShowAllowAutoRecertification, y => y.Ignore())
                .ForMember(x => x.MfaModeIsSet, y => y.Ignore());

            CreateMap<PersonDetailsBasicDto, PersonBasicModel>();

            CreateMap<PersonModel, UpdatePersonDetailsRequest>();

            CreateMap<PersonModel, UpdatePersonSettingsRequest>()
                .ForMember(x => x.RolePlayerSettingsRequest, y => y.Ignore());

            CreateMap<PersonNameDto, PersonNameModel>();

            CreateMap<PersonNameModel, PersonNameDto>();

            CreateMap <AddressModel, AddressDetailsDto>()
                .ForMember(x => x.Invalid, y => y.Ignore())
                .ForMember(x => x.DisplayName, y => y.Ignore());

            CreateMap <AddressDetailsDto, AddressModel>();

            CreateMap <PhoneModel, PhoneDetailsDto>()
                .ForMember(x => x.Invalid, y => y.Ignore());

            CreateMap <PhoneDetailsDto, PhoneModel>();

            CreateMap <EmailModel, EmailDetailsDto>()
                .ForMember(x => x.Invalid, y => y.Ignore());

            CreateMap <EmailDetailsDto, EmailModel>()
                .ForMember(x => x.ShowExaminer, y => y.Ignore());

            CreateMap <WebsiteModel, WebsiteDetailsDto>();

            CreateMap <WebsiteDetailsDto, WebsiteModel>();

            CreateMap <SuburbStatePostCodeDto, SuburbModel>();

            CreateMap <UpdatePhotoRequestModel, UpdatePhotoDto>()
                .ForMember(x => x.TokenToRemoveFromFilename, y => y.Ignore());

            CreateMap <PersonAttachmentModel, CreateOrReplacePersonAttachmentRequest>()
                .ForMember(x => x.TokenToRemoveFromFilename, y => y.Ignore());

            CreateMap <PersonAttachmentDto, PersonAttachmentModel>()
                .ForMember(x => x.Title, y => y.Ignore())
                .ForMember(x => x.NoteId, y => y.Ignore());
        }
    }
}
