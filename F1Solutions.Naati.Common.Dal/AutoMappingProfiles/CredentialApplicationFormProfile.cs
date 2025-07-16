using System.Collections.Generic;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class CredentialApplicationFormProfile : Profile
    {
        public CredentialApplicationFormProfile()
        {
            CreateMap<CredentialApplicationForm, CredentialApplicationFormDto>()
                .ForMember(
                    x => x.CredentialApplicationTypeName,y => y.MapFrom(z => z.CredentialApplicationType.DisplayName))
                .ForMember(x => x.FormUserTypeId, y => y.MapFrom(z => z.CredentialApplicationFormUserType.Id));

            CreateMap<CredentialApplicationFormSection, CredentialApplicationFormSectionDto>();

            CreateMap<CredentialApplicationFormQuestion, CredentialApplicationFormQuestionDto>()
                .ForMember(x => x.Text, y => y.MapFrom(z => z.CredentialApplicationFormQuestionType.Text))
                .ForMember(x => x.Description, y => y.MapFrom(z => z.CredentialApplicationFormQuestionType.Description))
                .ForMember(x => x.AnswerTypeId,y => y.MapFrom(z => z.CredentialApplicationFormQuestionType.CredentialApplicationFormAnswerType.Id))
                .ForMember(x => x.AnswerTypeName,y => y.MapFrom(z => z.CredentialApplicationFormQuestionType.CredentialApplicationFormAnswerType.Name))
                .ForMember(x => x.CredentialApplicationFieldId,y => y.MapFrom(z => Util.GetValueOrNull(z.CredentialApplicationField, w => w.Id)))
                .ForMember(x => x.AnswerOptions, y => y.MapFrom(z => z.QuestionAnswerOptions))
                .ForMember(x => x.Responses, y => y.MapFrom(z => new List<object>()));

            CreateMap<CredentialApplicationFormQuestionAnswerOption, CredentialApplicationQuestionAnswerOptionDto
                >()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.FormAnswerOptionId, y => y.MapFrom(z => z.CredentialApplicationFormAnswerOption.Id))
                .ForMember(x => x.CredentialApplicationFieldId,y => y.MapFrom(z => Util.GetValueOrNull(z.CredentialApplicationField, w => w.Id)))
                .ForMember(x => x.Description, y => y.MapFrom(z => z.CredentialApplicationFormAnswerOption.Description))
                .ForMember(x => x.Option, y => y.MapFrom(z => z.CredentialApplicationFormAnswerOption.Option))
                .ForMember(x => x.Actions, y => y.MapFrom(z => z.CredentialApplicationFormAnswerOption.Actions))
                .ForMember(x => x.Documents, y => y.MapFrom(z => z.CredentialApplicationFormAnswerOption.Documents))
                .ForMember(x => x.FieldOptionId, y => y.MapFrom(z => Util.GetValueOrNull(z.CredentialApplicationFieldOptionOption, w => w.Id)));

            CreateMap<CredentialApplicationFormAnswerOptionActionType, CredentialApplicationFormAnswerOptionActionTypeDto>()
                .ForMember(x => x.ActionTypeId, y => y.MapFrom(z => z.CredentialApplicationFormActionType.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.CredentialApplicationFormActionType.Name));

            CreateMap<CredentialApplicationFormAnswerOptionDocumentType, CredentialApplicationFormAnswerOptiondDocumentTypeDto>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.DocumentTypeId, y => y.MapFrom(z => z.DocumentType.Id))
                .ForMember(x => x.DisplayName, y => y.MapFrom(z => z.DocumentType.DisplayName));

            CreateMap<CredentialApplicationFormQuestionLogic, CredentialApplicationFormQuestionLogicDto>()
                .ForMember(x => x.QuestionAnswerOptionId,y => y.MapFrom(z => Util.GetValueOrNull(z.CredentialApplicationFormQuestionAnswerOption,w => w.Id)))
                .ForMember(x => x.CredentialTypeId, y => y.MapFrom(z => Util.GetValueOrNull(z.CredentialType, w => w.Id)))
                .ForMember(x => x.CredentialRequestPathTypeId,y => y.MapFrom(z => Util.GetValueOrNull(z.CredentialRequestPathType, w => w.Id)))
                .ForMember(x => x.SkillId, y => y.MapFrom(z => Util.GetValueOrNull(z.Skill, w => w.Id)));
        }
    }
}
