using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class EmailProfile : Profile
    {
        public EmailProfile()
        {
            CreateMap<EmailTemplateDto, EmailTemplateModel>()
                .ForMember(x => x.Content, y => y.Ignore())
                .ForMember(x => x.FromAddress, y => y.Ignore())
                .ForMember(x => x.EmailTemplateDetails, y => y.Ignore())
                .ForMember(x => x.SystemActionEventType, y => y.Ignore());

            CreateMap<EmailMessageDto, EmailMessageDto>();

            CreateMap<EmailTemplateDetailDto, EmailTemplateModel>();

            CreateMap<EmailMessageModel, EmailMessageModel>();

            CreateMap<WorkflowResponse, WorkflowModel>();

            CreateMap<EmailTemplateResponse, ServiceEmailTemplateModel>();

            CreateMap<LookupTypeModel, LookupTypeDto>()
                .ForMember(x => x.Name, y => y.Ignore())
                .ForMember(x => x.DisplayOrder, y => y.Ignore())
                .ForMember(x => x.ExtraData, y => y.Ignore());

            CreateMap<WorkflowModel, WorkflowResponse>();

            CreateMap<ServiceEmailTemplateModel, EmailTemplateRequest>()
                .ForMember(x => x.UserId, y => y.Ignore());

            CreateMap<MaterialRequestEmailMessageModel, MaterialRequestEmailMessageModel>();

            CreateMap<EmailMessageModel, EmailMessageDto>()
                .ForMember(x => x.Attachments, y => y.Ignore())
                .ForMember(x => x.HasAttachments, y => y.Ignore())
                .ForMember(x => x.CreatedUserName, y => y.Ignore());

            CreateMap<MaterialRequestEmailMessageModel, MaterialRequestEmailMessageDto>()
                .IncludeBase<EmailMessageModel, EmailMessageDto>()
                .ForMember(x => x.CreatedUserName, y => y.Ignore());

            CreateMap<CredentialApplicationEmailMessageModel, ApplicationEmailMessageDto>()
                .IncludeBase<EmailMessageModel, EmailMessageDto>()
                .ForMember(x => x.CreatedUserName, y => y.Ignore());

            CreateMap<EmailMessageDto, CredentialApplicationEmailMessageModel>()
                .ForMember(x => x.CredentialApplicationId, y => y.Ignore())
                .ForMember(x => x.EmailSendStatus, y => y.Ignore());

            CreateMap<EmailMassageHeaderDto, EmailMassageHeaderModel>();

            CreateMap<EmailMessageAttachmentDto, EmailMessageAttachmentModel>()
                .ForMember(x => x.AttachmentType, y => y.Ignore())
                .ForMember(x => x.Title, y => y.Ignore())
                .ForMember(x => x.Type, y => y.Ignore());

            CreateMap<EmailMessageDto, MaterialRequestEmailMessageModel>()
                .ForMember(x => x.MaterialRequestId, y => y.Ignore())
                .ForMember(x => x.EmailSendStatus, y => y.Ignore());
        }
    }
}
