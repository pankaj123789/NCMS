using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class EmailProfile : Profile
    {
        public EmailProfile()
        {
            CreateMap<EmailMessageAttachment, EmailMessageAttachmentDto>()
                .ForMember(x => x.StoredFileId, y => y.MapFrom(x => x.StoredFile.Id))
                .ForMember(x => x.DocumentType, y => y.MapFrom(x => x.StoredFile.DocumentType.Name))
                .ForMember(x => x.FileName, y => y.MapFrom(x => x.FileName))
                .ForMember(x => x.FileSize, y => y.MapFrom(x => x.StoredFile.FileSize))
                .ForMember(x => x.EmailMessageAttachmentId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.GraphAttachmentId, y => y.Ignore())
                .ForMember(x => x.GraphAttachmentSize, y => y.Ignore())
                .ForMember(x => x.FilePath, y => y.Ignore())
                .ForMember(x => x.StoragePath, y => y.Ignore())
                .ForMember(x => x.UploadedByName, y => y.Ignore())
                .ForMember(x => x.UploadedByUserId, y => y.Ignore())
                .ForMember(x => x.UploadedDateTime, y => y.Ignore())
                .ForMember(x => x.FileType, y => y.Ignore())
                .ForMember(x => x.GraphAttachmentBytes, y => y.Ignore())
                .ForMember(x => x.IsInline, y => y.Ignore())
                .ForMember(x => x.ContentId, y => y.Ignore());

            CreateMap<EmailMessage, EmailMessageDto>()
                .ForMember(x => x.EmailMessageId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.CreatedUserId, y => y.MapFrom(x => Util.GetValueOrNull(x.CreatedUser, w => w.Id)))
                .ForMember(x => x.RecipientEntityId, y => y.MapFrom(x => Util.GetValueOrNull(x.RecipientEntity, w => w.Id)))
                .ForMember(x => x.RecipientUserId, y => y.MapFrom(x => Util.GetValueOrNull(x.RecipientUser, w => w.Id)))
                .ForMember(x => x.From, y => y.MapFrom(z => z.FromAddress))
                .ForMember(x => x.GraphEmailMessageId, y => y.Ignore())
                .ForMember(x => x.HasAttachments, y => y.MapFrom(z => z.Attachments.Any()))
                .ForMember(x => x.CreatedUserName, y => y.MapFrom(z => z.CreatedUser.UserName))
                .ForMember(x => x.Attachments, y => y.MapFrom(z => z.Attachments))
                .ForMember(x => x.EmailTemplateId, y => y.MapFrom(z => z.EmailTemplate.Id));

            CreateMap<EmailMessage, ApplicationEmailMessageDto>()
                .ForMember(x => x.CredentialApplicationId, y => y.Ignore())
                .IncludeBase<EmailMessage, EmailMessageDto>();

            CreateMap<EmailMessage, MaterialRequestEmailMessageDto>()
                .ForMember(x => x.MaterialRequestId, y => y.Ignore())
                .IncludeBase<EmailMessage, EmailMessageDto>();

            CreateMap<EmailMessageDto, EmailMessage>()
                .ForMember(x => x.Id, y => y.MapFrom(x => x.EmailMessageId))
                .ForMember(x => x.FromAddress, y => y.MapFrom(x => x.From))
                .ForMember(x => x.RecipientEntity, y => y.Ignore())
                .ForMember(x => x.RecipientUser, y => y.Ignore())
                .ForMember(x => x.CreatedUser, y => y.Ignore())
                .ForMember(x => x.CreatedDate, y => y.Ignore())
                .ForMember(x => x.EmailSendStatusType, y => y.Ignore())
                .ForMember(x => x.EmailTemplate, y => y.Ignore());

            CreateMap<EmailMessageResponse, EmailMessageDto>()
                .ForMember(x => x.GraphEmailMessageId, y => y.MapFrom(z => z.Data.GraphEmailMessageId))
                .ForMember(x => x.EmailMessageId, y => y.MapFrom(z => z.Data.EmailMessageId))
                .ForMember(x => x.RecipientEntityId, y => y.MapFrom(z => z.Data.RecipientEntityId))
                .ForMember(x => x.RecipientUserId, y => y.MapFrom(z => z.Data.RecipientUserId))
                .ForMember(x => x.RecipientEmail, y => y.MapFrom(z => z.Data.RecipientEmail))
                .ForMember(x => x.Subject, y => y.MapFrom(z => z.Data.Subject))
                .ForMember(x => x.Body, y => y.MapFrom(z => z.Data.Body))
                .ForMember(x => x.CreatedUserId, y => y.MapFrom(z => z.Data.CreatedUserId))
                .ForMember(x => x.Bcc, y => y.MapFrom(z => z.Data.Bcc))
                .ForMember(x => x.From, y => y.MapFrom(z => z.Data.From))
                .ForMember(x => x.Cc, y => y.MapFrom(z => z.Data.Cc))
                .ForMember(x => x.LastSendAttemptDate, y => y.MapFrom(z => z.Data.LastSendAttemptDate))
                .ForMember(x => x.CreatedDate, y => y.MapFrom(z => z.Data.CreatedDate))
                .ForMember(x => x.LastSendResult, y => y.MapFrom(z => z.Data.LastSendResult))
                .ForMember(x => x.Attachments, y => y.MapFrom(z => z.Data.Attachments))
                .ForMember(x => x.HasAttachments, y => y.MapFrom(z => z.Data.HasAttachments))
                .ForMember(x => x.CreatedUserName, y => y.MapFrom(z => z.Data.CreatedUserName))
                .ForMember(x => x.EmailSendStatusTypeId, y => y.MapFrom(z => z.Data.EmailSendStatusTypeId))
                .ForMember(x => x.EmailTemplateId, y => y.MapFrom(z => z.Data.EmailTemplateId));

            CreateMap<EmailTemplate, EmailTemplateDto>();

            CreateMap<EmailDetailsDto, Email>()
                .ForMember(x => x.EmailAddress, y => y.Ignore())
                .ForMember(x => x.Entity, y => y.Ignore())
                .ForMember(x => x.Id, y => y.Ignore());
        }
    }
}
