using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using IPersonService = Ncms.Contracts.IPersonService;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Bl
{
    public class EmailMessageService : IEmailMessageService
    {
        private readonly IEmailMessageQueryService _emailMessageQueryService;
        private readonly IPersonService _personService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public EmailMessageService(IEmailMessageQueryService emailMessageQueryService,
            IPersonService personService, IAutoMapperHelper autoMapperHelper)
        {
            _emailMessageQueryService = emailMessageQueryService;
            _personService = personService;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<EmailMessageModel> GetGenericEmailMessage(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var emailResponse = _emailMessageQueryService.GetEmailMessage(id);

            var emailModel = MapToModel<EmailMessageModel>(emailResponse.Data);

            return emailModel;
        }

        public GenericResponse<IEnumerable<EmailQueueEmailMessageModel>> GetEmailMessageQueue(SearchRequest request)
        {
            var filters = request.Filter.ToFilterList<EmailSearchCriteria, EmailFilterType>();
            var getRequest = _autoMapperHelper.Mapper.Map<GetEmailSearchRequest>(request);
            getRequest.Filters = filters;

            var emailList = _emailMessageQueryService.SearchEmail(getRequest);

           var models = emailList.Data.Select(x => new EmailQueueEmailMessageModel
           {
               EmailMessageId = x.EmailMessageId,
               CreatedDate = x.CreatedDate,
               CreatedUserDisplayName = x.CreatedUserName,
               Subject = x.Subject,
               RecipientEmail = x.RecipientEmail,
               LastSendAttemptDate = x.LastSendAttemptDate,
               LastSendResult = x.LastSendResult,
               EmailSendStatus = x.EmailSendStatus
           });

            return new GenericResponse<IEnumerable<EmailQueueEmailMessageModel>>(models);
        }
        
        public GenericResponse<IEnumerable<EmailMessageAttachmentModel>> ListAttachments(int emailMessageId)
        {
            var request = new GetEmailMessageAttachmentsRequest
            {
                EmailMessageId = emailMessageId,
            };

            GetEmailMessageAttachmentsResponse response = null;

            try
            {
                response = _emailMessageQueryService.GetAttachments(request);

            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return new GenericResponse<IEnumerable<EmailMessageAttachmentModel>>(response?.Attachments.Select(a =>
            {
                var model = _autoMapperHelper.Mapper.Map<EmailMessageAttachmentModel>(a);
                model.FileType = Path.GetExtension(a.FileName)?.Trim('.');
                model.Title = a.Description;
                return model;
            }).ToArray() ?? new EmailMessageAttachmentModel[0]);
        }

        public GenericResponse<CredentialApplicationEmailMessageModel> CreateEmailMessage(CredentialApplicationEmailMessageModel model)
        {
            return CreateEmailMessage(model, SaveApplicationMessage);
        }
        public GenericResponse<MaterialRequestEmailMessageModel> CreateEmailMessage(MaterialRequestEmailMessageModel model)
        {
            return CreateEmailMessage(model, SaveMaterialRequestMessage);
        }

        public GenericResponse<EmailMessageModel> CreateGenericEmailMessage(EmailMessageModel model)
        {
            return CreateEmailMessage(model, SaveGenericEmailMessage);
        }


        public GenericResponse<T> CreateEmailMessage<T>(T model,  Func <T, T> saveAction) where T :EmailMessageModel, new()
        {
            return saveAction(model);
        }

        private T SaveApplicationMessage<T>(T model) where T : CredentialApplicationEmailMessageModel, new()
        {
            var dto = MapToDto<T, ApplicationEmailMessageDto >(model);
            var emailResponse = _emailMessageQueryService.CreateApplicationEmailMessage(new ApplicationEmailMessageRequest { EmailMessage = dto, FailureStatus = EmailSendStatusTypeName.Retry });

            T result = MapToModel<T>(emailResponse.Data);
            result.CredentialApplicationId =
           emailResponse.Data.CredentialApplicationId;

            return result;
        }

        private T SaveGenericEmailMessage<T>(T model) where T : EmailMessageModel, new()
        {
            var dto = MapToDto<T, EmailMessageDto>(model);
            var emailResponse = _emailMessageQueryService.CreateGenericEmailMessage(new GenericEmailMessageRequest { EmailMessage = dto, FailureStatus = EmailSendStatusTypeName.Retry });

            T result = MapToModel<T>(emailResponse.Data);
            return result;
        }

        private T SaveMaterialRequestMessage<T>(T model) where T : MaterialRequestEmailMessageModel, new()
        {
            var dto = MapToDto<T, MaterialRequestEmailMessageDto>(model);
            var emailResponse = _emailMessageQueryService.CreateMaterialRequestEmailMessage(new MaterialRequestEmailMessageRequest { EmailMessage = dto, FailureStatus = EmailSendStatusTypeName.Retry });

            T result = MapToModel<T>(emailResponse.Data);
            result.MaterialRequestId =emailResponse.Data.MaterialRequestId;

            return result;
        }

        public BusinessServiceResponse SendEmailMessage(int emailMessageId)
        {
            var email = _emailMessageQueryService.GetEmailMessage(emailMessageId).Data;
            var primaryEmail = _personService.GetPersonDetailsByEntityId(email.RecipientEntityId)?.PersonDetails?.PrimaryEmail;
            var failureStatus = email.EmailSendStatusTypeId == (int)EmailSendStatusTypeName.Failed ? EmailSendStatusTypeName.Failed : EmailSendStatusTypeName.Retry;

            if (string.Compare(email.RecipientEmail, primaryEmail, StringComparison.OrdinalIgnoreCase) == 0)
            {
                var serviceResponse = _emailMessageQueryService.SendMailById(emailMessageId, failureStatus);
                return serviceResponse.ConvertServiceResponse();
            }
            else
            {
                var dto = _autoMapperHelper.Mapper.Map<EmailMessageDto>(email);
                dto.RecipientEmail = primaryEmail;
               
                var serviceResponse = _emailMessageQueryService.SendMail(new GenericEmailMessageRequest { EmailMessage = dto, FailureStatus = failureStatus });
                return serviceResponse.ConvertServiceResponse();
            }
        }

        public GenericResponse<IEnumerable<CredentialApplicationEmailMessageModel>> GetEmailMessageByCredentialApplicationId(int creadentialApplicationId)
        {
            if(creadentialApplicationId <= 0)
                throw new ArgumentOutOfRangeException(nameof(creadentialApplicationId));

            var emailResponse = _emailMessageQueryService.GetEmailMessageByCredentialApplicationId(creadentialApplicationId);

            return emailResponse.ConvertServiceListResponse<EmailMessageDto, CredentialApplicationEmailMessageModel>();
        }

        public BusinessServiceResponse SendEmailMessageById(int emailMessageId)
        {
            var email = _emailMessageQueryService.GetEmailMessage(emailMessageId).Data;
            var failureStatus = email.EmailSendStatusTypeId == (int)EmailSendStatusTypeName.Failed ? EmailSendStatusTypeName.Failed : EmailSendStatusTypeName.Retry;

            if (emailMessageId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(emailMessageId));
            }

            var serviceResponse = _emailMessageQueryService.SendMailById(emailMessageId, failureStatus);
            return serviceResponse.ConvertServiceResponse();
        }

        public GenericResponse<IEnumerable<EmailMassageHeaderModel>> GetMailMessagesFromSharedAccount(List<string> sharedAccount, int naatiNumber)
        {
            var userEmail = _personService.GetPersonBasic(naatiNumber).Data.PrimaryEmail;
            var emailResponse = _emailMessageQueryService.GetMailMessagesFromSharedAccount(sharedAccount, userEmail);

            return emailResponse.ConvertServiceListResponse<EmailMassageHeaderDto, EmailMassageHeaderModel>();
        }

        public GenericResponse<EmailMessageModel> GetEmailDetails(string graphEmailId, string userEmail)
        {
         
            var emailResponse = _emailMessageQueryService.GetMailDetails(graphEmailId, userEmail);

            var result = MapToModel<EmailMessageModel>(emailResponse.Data);

            return new GenericResponse<EmailMessageModel>(result);
        }

        public GenericResponse<IEnumerable<EmailMessageAttachmentModel>> GetMailAttachment(string graphEmailId, string userEmail)
        {
            var attachments = _emailMessageQueryService.GetMailAttachment(graphEmailId, userEmail);

            return attachments.ConvertServiceListResponse<EmailMessageAttachmentDto, EmailMessageAttachmentModel>();
        }

        public GetEmailTemplateResponse GetSystemEmailTemplate(GetSystemEmailTemplateRequest request)
        {
            return _emailMessageQueryService.GetSystemEmailTemplate(request);
        }

        /// <summary>
        /// This was written due to repeated issues with AutoMapper in Test/UAT/Prod
        /// </summary>
        private TR MapToDto<T,TR>(T model) where T : EmailMessageModel where TR : EmailMessageDto, new()
        {
            //if (model.RecipientEntityId <= 0 && model.RecipientUserId <= 0)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(model.RecipientEntityId));
            //}

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (model.EmailTemplateId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EmailTemplateId));
            }

            var mappedMessage = _autoMapperHelper.Mapper.Map<TR>(model);

            mappedMessage.Attachments = model.Attachments?.Select(x => new EmailMessageAttachmentDto
                {
                    ContentId = x.ContentId,
                    Description = x.Title,
                    DocumentType = x.DocumentType,
                    EmailMessageAttachmentId = x.EmailMessageAttachmentId,
                    EmailMessageId = x.EmailMessageId,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    FileSize = x.FileSize,
                    FileType = x.FileType,
                    GraphAttachmentBytes = x.GraphAttachmentBytes,
                    GraphAttachmentSize = x.GraphAttachmentSize,
                    IsInline = x.IsInline,
                    StoragePath = x.StoragePath,
                    StoredFileId = x.StoredFileId,
                    UploadedByName = x.UploadedByName,
                    UploadedByUserId = x.UploadedByUserId,
                    UploadedDateTime = x.UploadedDateTime
                })
                .ToArray();
            mappedMessage.HasAttachments = model.Attachments?.Any() == true;
            return mappedMessage;

        }

        /// <summary>
        /// This was written due to repeated issues with AutoMapper in Test/UAT/Prod
        /// </summary>
        private T MapToModel<T>(EmailMessageDto dto)
            where T : EmailMessageModel, new()
        {
            T model = new T
            {
                Attachments = dto.Attachments?.Select(x => new EmailMessageAttachmentModel
                {
                    ContentId = x.ContentId,
                    DocumentType = x.DocumentType,
                    EmailMessageAttachmentId = x.EmailMessageAttachmentId,
                    EmailMessageId = x.EmailMessageId,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    FileSize = x.FileSize,
                    FileType = x.FileType,
                    GraphAttachmentBytes = x.GraphAttachmentBytes,
                    GraphAttachmentSize = x.GraphAttachmentSize,
                    IsInline = x.IsInline,
                    StoragePath = x.StoragePath,
                    StoredFileId = x.StoredFileId,
                    Title = x.Description,
                    UploadedByName = x.UploadedByName,
                    UploadedByUserId = x.UploadedByUserId,
                    UploadedDateTime = x.UploadedDateTime,
                }).ToArray(),
                Bcc = dto.Bcc,
                Body = dto.Body,
                Cc = dto.Cc,
                CreatedDate = dto.CreatedDate,
                CreatedUserId = dto.CreatedUserId,
                EmailMessageId = dto.EmailMessageId,
                From = dto.From,
                GraphEmailMessageId = dto.GraphEmailMessageId,
                LastSendAttemptDate = dto.LastSendAttemptDate,
                LastSendResult = dto.LastSendResult,
                RecipientEmail = dto.RecipientEmail,
                HasAttachments = dto.Attachments?.Any() == true,
                RecipientEntityId = dto.RecipientEntityId,
                RecipientUserId = dto.RecipientUserId,
                Subject = dto.Subject,
                EmailSendStatusTypeId = dto.EmailSendStatusTypeId,
                EmailTemplateId = dto.EmailTemplateId
            };
            return model;
        }

        public GenericResponse<IEnumerable<MaterialRequestEmailMessageModel>> GetEmailMessageByMaterialRequestId(int materialRequestId)
        {
            if (materialRequestId <= 0)
                throw new ArgumentOutOfRangeException(nameof(materialRequestId));

            var emailResponse = _emailMessageQueryService.GetEmailMessageByMaterialRequestId(materialRequestId);

            return emailResponse.ConvertServiceListResponse<EmailMessageDto, MaterialRequestEmailMessageModel>();
        }
    }
}
