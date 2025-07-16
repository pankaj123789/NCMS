using System;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts;
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
    public class CredentialApplicationProfile : Profile
    {
        public CredentialApplicationProfile()
        {
            CreateMap<CredentialApplicationFieldDataDto, ApplicationFieldData>();
            CreateMap<CredentialApplicationFormQuestionDto, ApplicationFormQuestionContract>()
                .ForMember(x => x.Response, y => y.Ignore())
                .ForMember(x => x.HasTokens, y => y.Ignore());

            CreateMap<AnswerDocumentContract, CredentialApplicationFormAnswerOptiondDocumentTypeDto>();
                
            CreateMap<AnswerOptionContract, CredentialApplicationQuestionAnswerOptionDto>()
                .ForMember(x => x.Actions, y => y.Ignore());

            CreateMap<QuestionLogicContract, CredentialApplicationFormQuestionLogicDto>()
                .ForMember(x => x.QuestionAnswerOptionId, y => y.Ignore())
                .ForMember(x => x.CredentialTypeId, y => y.Ignore())
                .ForMember(x => x.CredentialRequestPathTypeId, y => y.Ignore());

            CreateMap<ApplicationFormQuestionContract, CredentialApplicationFormQuestionDto>();

            CreateMap<CredentialApplicationQuestionAnswerOptionDto, AnswerOptionContract>()
                .ForMember(x => x.Function, y => y.MapFrom(z => z.Actions.FirstOrDefault()))
                .ForMember(x => x.HasTokens, y => y.Ignore());

            CreateMap<CredentialApplicationFormAnswerOptionActionTypeDto, OptionActionContract>();

            CreateMap<CredentialApplicationFormAnswerOptiondDocumentTypeDto, AnswerDocumentContract>();

            CreateMap<CredentialApplicationFormQuestionLogicDto, QuestionLogicContract>()
                .ForMember(l => l.AnswerId, y => y.MapFrom(dto => dto.CredentialTypeId ?? dto.CredentialRequestPathTypeId ?? dto.QuestionAnswerOptionId.GetValueOrDefault()))
                .ForMember(l => l.Type, y => y.MapFrom(dto => dto.CredentialTypeId.HasValue ? QuestionLogicType.CredentialType : (dto.CredentialRequestPathTypeId.HasValue ? QuestionLogicType.CredentialRequestPathType : (dto.PdPointsMet.HasValue ? QuestionLogicType.PdPonints : (dto.WorkPracticeMet.HasValue ? QuestionLogicType.WorkPractice : (dto.SkillId.HasValue ? QuestionLogicType.Skill : QuestionLogicType.AnswerOption))))));

            CreateMap<CredentialApplicationFormSectionDto, ApplicationFormSectionContract>()
                .ForMember(x => x.HasTokens, y => y.Ignore());

            CreateMap<CredentialApplicationDto, UpsertCredentialApplicationRequest>()
                .ForMember(x => x.Fields, y => y.Ignore())
                .ForMember(x => x.CredentialRequests, y => y.Ignore())
                .ForMember(x => x.Notes, y => y.Ignore())
                .ForMember(x => x.PersonNotes, y => y.Ignore())
                .ForMember(x => x.StandardTestComponents, y => y.Ignore())
                .ForMember(x => x.RubricTestComponents, y => y.Ignore())
                .ForMember(x => x.PdActivities, y => y.Ignore())
                .ForMember(x => x.Recertification, y => y.Ignore())
                .ForMember(x => x.ProcessedFees, y => y.Ignore())
                .ForMember(x => x.Attachments, y => y.Ignore());


            CreateMap<CredentialApplicationFieldDataDto, CredentialApplicationFieldData>()
                .ForMember(x => x.CredentialApplication, y => y.Ignore())
                .ForMember(x => x.CredentialApplicationField, y => y.Ignore())
                .ForMember(x => x.CredentialApplicationFieldOptionOption, y => y.Ignore());

            CreateMap<CreateOrReplaceAttachmentContract, CreateOrReplaceApplicationAttachmentRequest>();

            CreateMap<CredentialApplicationAttachmentDalModel, CreateOrReplaceApplicationAttachmentRequest>()
                .ForMember(x => x.TokenToRemoveFromFilename, y => y.Ignore());

            CreateMap<CredentialApplicationFormDto, ApplicationFormContract>();

            CreateMap<BasicApplicationSearchDto, CredentialApplicationContract>()
                .ForMember(x => x.Sections, y => y.Ignore());
        }
    }
}
