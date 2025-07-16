using System;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using PaymentDto = MyNaati.Contracts.BackOffice.PaymentDto;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class CredentialProfile : Profile
    {
        public CredentialProfile()
        {
            CreateMap<CredentialRequestFieldDataDto, CredentialFieldData>()
                .ForMember(x => x.FieldTypeId, y => y.Ignore());

            CreateMap<CredentialRequestDto, CredentialRequestData>()
                .ForMember(x => x.CredentialId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.WorkPractices, y => y.Ignore())
                .ForMember(x => x.Briefs, y => y.Ignore());

            CreateMap<CredentialDto, CredentialContract>();

            CreateMap<CredentialRequestDto, CredentialRequestContract>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Category, y => y.MapFrom(z => z.Category))
                .ForMember(x => x.Level, y => y.MapFrom(z => z.ExternalCredentialName))
                .ForMember(x => x.Skill, y => y.MapFrom(z => z.Direction))
                .ForMember(x => x.SkillId, y => y.MapFrom(z => z.SkillId))
                .ForMember(x => x.CategoryId, y => y.MapFrom(z => z.CategoryId))
                .ForMember(x => x.LevelId, y => y.MapFrom(z => z.CredentialTypeId))
                .ForMember(x => x.PathTypeId, y => y.MapFrom(z => z.CredentialRequestPathTypeId))
                .ForMember(x => x.ApplicationTypeDisplayName, y => y.MapFrom(z => z.ApplicationTypeDisplayName))
                .ForMember(x => x.ApplicationReference, y => y.Ignore());

            CreateMap<CredentialAttachmentDto, CredentialAttachmentContract>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.GetCredentialAttachmentFileResponse, GetCredentialAttachmentFileResponse>();

            CreateMap<CredentialContract, CredentialDto>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.ShowInOnlineDirectory, y => y.MapFrom(z => z.ShowInOnlineDirectory))
                .ForMember(x => x.CredentialTypeInternalName, y => y.MapFrom(z => z.CredentialTypeInternalName))
                .ForMember(x => x.CredentialTypeExternalName, y => y.MapFrom(z => z.CredentialTypeExternalName))
                .ForMember(x => x.ExpiryDate, y => y.MapFrom(z => z.ExpiryDate))
                .ForMember(x => x.SkillDisplayName, y => y.MapFrom(z => z.SkillDisplayName))
                .ForMember(x => x.StatusId, y => y.Ignore())
                .ForMember(x => x.TerminationDate, y => y.Ignore())
                .ForMember(x => x.CredentialTypeId, y => y.Ignore())
                .ForMember(x => x.CategoryId, y => y.Ignore())
                .ForMember(x => x.StoredFileIds, y => y.Ignore())
                .ForMember(x => x.CertificationPeriod, y => y.Ignore());
        }
    }
}
