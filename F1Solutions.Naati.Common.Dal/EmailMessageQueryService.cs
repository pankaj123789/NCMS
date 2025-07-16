using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.Office365;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using Microsoft.Graph;
using Attachment = System.Net.Mail.Attachment;
using User = F1Solutions.Naati.Common.Dal.Domain.User;

namespace F1Solutions.Naati.Common.Dal
{
    public class EmailMessageQueryService : IEmailMessageQueryService
    {
        private readonly ISystemQueryService _systemQueryService;
        private readonly ISecretsCacheQueryService _secretsProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public EmailMessageQueryService(ISystemQueryService systemQueryService, ISecretsCacheQueryService secretsProvider, IAutoMapperHelper autoMapperHelper)
        {
            _systemQueryService = systemQueryService;
            _secretsProvider = secretsProvider;
            _autoMapperHelper = autoMapperHelper;
        }

        private T MapEmailMessage<T>(EmailMessage emailMessage) where T : EmailMessageDto
        {
            var dto = _autoMapperHelper.Mapper.Map<T>(emailMessage);

            dto.From = string.IsNullOrWhiteSpace(emailMessage.FromAddress)
                ? NHibernateSession.Current.Query<SystemValue>()
                    .FirstOrDefault(x => x.ValueKey == "DefaultEmailSenderAddress")?.Value
                : emailMessage.FromAddress;

            return dto;
        }


        public ApplicationEmailMessageResponse CreateApplicationEmailMessage(ApplicationEmailMessageRequest request)
        {
            var applicationMessage = new CredentialApplicationEmailMessage
            {
                CredentialApplication =
                    NHibernateSession.Current.Load<CredentialApplication>(request.EmailMessage.CredentialApplicationId),
                EmailMessage = PrepareEmailMessageToUpdate(request)
            };


            NHibernateSession.Current.Save(applicationMessage);
            NHibernateSession.Current.Flush();

            var dto = MapEmailMessage<ApplicationEmailMessageDto>(applicationMessage.EmailMessage);
            dto.CredentialApplicationId = applicationMessage.CredentialApplication.Id;

            return new ApplicationEmailMessageResponse { Data = dto };

        }

        public MaterialRequestEmailMessageResponse CreateMaterialRequestEmailMessage(MaterialRequestEmailMessageRequest request)
        {
            var materialRequestMessage = new MaterialRequestEmailMessage()
            {
                MaterialRequest = NHibernateSession.Current.Load<Domain.MaterialRequest>(request.EmailMessage.MaterialRequestId),
                EmailMessage = PrepareEmailMessageToUpdate(request)
            };

            NHibernateSession.Current.Save(materialRequestMessage);
            NHibernateSession.Current.Flush();

            var dto = MapEmailMessage<MaterialRequestEmailMessageDto>(materialRequestMessage.EmailMessage);
            dto.MaterialRequestId = materialRequestMessage.MaterialRequest.Id;

            return new MaterialRequestEmailMessageResponse { Data = dto };

        }

        public EmailMessageResponse CreateGenericEmailMessage(GenericEmailMessageRequest request)
        {
            var message = PrepareEmailMessageToUpdate(request);

            NHibernateSession.Current.Save(message);
            NHibernateSession.Current.Flush();

            var dto = MapEmailMessage<EmailMessageDto>(message);
            return new EmailMessageResponse { Data = dto };

        }



        private EmailMessage PrepareEmailMessageToUpdate<R>(EmailMessageRequest<R> request) where R : EmailMessageDto
        {
            var emailMessage = new EmailMessage();
            var dto = request.EmailMessage;

            //if (dto.RecipientEntityId <= 0 && dto.RecipientUserId <= 0)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(dto.RecipientEntityId));
            //}

            if (dto.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dto.CreatedUserId));
            }

            if (dto.EmailTemplateId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dto.EmailTemplateId));
            }

            _autoMapperHelper.Mapper.Map(dto, emailMessage);

            if (dto.RecipientEntityId == 0)
            {
                emailMessage.RecipientEntity = null;
            }
            if (dto.RecipientEntityId > 0)
            {
                emailMessage.RecipientEntity = NHibernateSession.Current.Get<NaatiEntity>(dto.RecipientEntityId);
            }

            if (dto.RecipientUserId == 0)
            {
                emailMessage.RecipientUser = null;
            }
            if (dto.RecipientUserId > 0)
            {
                emailMessage.RecipientUser = NHibernateSession.Current.Get<User>(dto.RecipientUserId);
            }

            emailMessage.EmailSendStatusType = NHibernateSession.Current.Get<EmailSendStatusType>(dto.EmailSendStatusTypeId);

            emailMessage.CreatedUser = NHibernateSession.Current.Get<User>(dto.CreatedUserId);

            emailMessage.CreatedDate = DateTime.Now;

            emailMessage.EmailTemplate = NHibernateSession.Current.Get<EmailTemplate>(dto.EmailTemplateId);

            if (dto.Attachments != null)
            {
                foreach (var attachmentDto in dto.Attachments)
                {
                    var storedFile = NHibernateSession.Current.Get<StoredFile>(attachmentDto.StoredFileId);
                    if (storedFile == null)
                    {
                        throw new Exception("Todo: support email attachments that haven't been stored yet");
                    }

                    var fileName = !string.IsNullOrWhiteSpace(attachmentDto.FileName)
                        ? Path.GetFileNameWithoutExtension(attachmentDto.FileName)
                        : Path.GetFileNameWithoutExtension(storedFile.FileName) ?? DateTime.Now.Ticks.ToString();

                    var attachment = new EmailMessageAttachment
                    {
                        Description = attachmentDto.Description,
                        StoredFile = storedFile,
                        EmailMessage = emailMessage,
                        FileName = $"{fileName}{Path.GetExtension(storedFile.FileName)}"
                    };

                    emailMessage.AddAttachment(attachment);
                }
            }

            NHibernateSession.Current.Save(emailMessage);
            NHibernateSession.Current.Flush();

            return emailMessage;
        }

        public GetEmailMessageAttachmentsResponse GetAttachments(GetEmailMessageAttachmentsRequest request)
        {
            var query = NHibernateSession.Current.Query<EmailMessageAttachment>();
            var attachments =
                query.Where(n => n.EmailMessage.Id == request.EmailMessageId)
                    .ToList()
                    .Select(n => new EmailMessageAttachmentDto
                    {
                        EmailMessageAttachmentId = n.Id,
                        EmailMessageId = n.EmailMessage.Id,
                        StoredFileId = n.StoredFile.Id,
                        FileName = n.StoredFile.FileName,
                        Description = n.Description,
                        DocumentType = n.StoredFile.DocumentType.DisplayName,
                        UploadedByName = n.StoredFile.UploadedByUser.FullName,
                        UploadedDateTime = n.StoredFile.UploadedDateTime,
                        FileSize = n.StoredFile.FileSize,
                    });

            return new GetEmailMessageAttachmentsResponse
            {
                Attachments = attachments.ToArray()
            };
        }

        public EmailMessageResponse GetEmailMessage(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var emailMessage = NHibernateSession.Current.Get<EmailMessage>(id);

            if (emailMessage == null)
            {
                throw new WebServiceException($"Email Message not found (Email Message ID {id})");
            }

            return new EmailMessageResponse { Data = MapEmailMessage<EmailMessageDto>(emailMessage) };
        }
        public void UpdateEmailMessageDetails(EmailMessageDto email)
        {
            var emailMessage = NHibernateSession.Current.Get<EmailMessage>(email.EmailMessageId);
            var emailSendStatusType = NHibernateSession.Current.Get<EmailSendStatusType>(email.EmailSendStatusTypeId);

            if (emailMessage == null)
            {
                throw new WebServiceException(
                    $"Email Message not found (Email Message ID {email.EmailMessageId})");
            }

            emailMessage.RecipientEmail = email.RecipientEmail;
            emailMessage.LastSendAttemptDate = email.LastSendAttemptDate;
            emailMessage.LastSendResult = email.LastSendResult;
            emailMessage.EmailSendStatusType = emailSendStatusType;

            NHibernateSession.Current.Save(emailMessage);
            NHibernateSession.Current.Flush();
        }

        public ServiceResponse SendMailById(int emailMessageId, EmailSendStatusTypeName failureStatus)
        {
            var dto = _autoMapperHelper.Mapper.Map<EmailMessageDto>(GetEmailMessage(emailMessageId).Data);
            return SendMail(new GenericEmailMessageRequest { EmailMessage = dto, FailureStatus = failureStatus });
        }

        public ServiceResponse SendMail(GenericEmailMessageRequest request)
        {
            var useOffice365ToSendEmail =
                _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "UseOffice365ToSendEmail" }).Value;

            if (Convert.ToBoolean(useOffice365ToSendEmail))
            {
                return SendEmailUsingOffice365(request, true);
            }

            return SendMail(request, true);
        }

        public ServiceResponse ProcessPendingEmail(GenericEmailMessageRequest emailRequest)
        {
            var useOffice365ToSendEmail = _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "UseOffice365ToSendEmail" }).Value;

            if (Convert.ToBoolean(useOffice365ToSendEmail))
            {
                return SendEmailUsingOffice365(emailRequest, true);
            }

            return SendMail(emailRequest, true);
        }

        private void MarkEmailAsSending(EmailMessageDto email)
        {
            email.RecipientEmail = GetRecipientEmail(email);
            email.EmailSendStatusTypeId = (int)EmailSendStatusTypeName.Sending;
            email.LastSendAttemptDate = DateTime.Now;
            UpdateEmailMessageDetails(email);
        }

        private string GetRecipientEmail(EmailMessageDto email)
        {
            if (email.RecipientEntityId > 0)
            {
                return NHibernateSession.Current.Query<Email>()
                           .FirstOrDefault(x => x.Entity.Id == email.RecipientEntityId && x.IsPreferredEmail &&
                                                !x.Invalid)
                           ?.EmailAddress ?? email.RecipientEmail;
            }
            return NHibernateSession.Current.Query<User>().FirstOrDefault(x => x.Id == email.RecipientUserId)?.Email ?? email.RecipientEmail;

        }

        private void MarkEmailAsSuccessful(EmailMessageDto email)
        {
            email.LastSendResult = "Successful";
            email.EmailSendStatusTypeId = (int)EmailSendStatusTypeName.Successful;
            UpdateEmailMessageDetails(email);
        }

        private void MarkEmailAsFailed(EmailMessageDto email, EmailSendStatusTypeName status, string lastSendResult)
        {
            email.LastSendResult = lastSendResult;
            email.EmailSendStatusTypeId = (int)status;
            UpdateEmailMessageDetails(email);
        }

        private ServiceResponse SendEmailUsingOffice365(GenericEmailMessageRequest request, bool updateDetails)
        {
            var response = new ServiceResponse();

            try
            {
                if (updateDetails)
                {
                    MarkEmailAsSending(request.EmailMessage);
                }

                var message = MapEmailMessage(request.EmailMessage);
                var authenticator = new AzureAuthorisationService(_systemQueryService, _secretsProvider);
                var o365 = new Office365IntegrationService(authenticator);

                var sendEmailTask = o365.SendEmail(message, saveEmail: updateDetails);
                sendEmailTask.Wait();
                var result = sendEmailTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    if (updateDetails)
                    {
                        MarkEmailAsSuccessful(request.EmailMessage);
                    }
                }
                else
                {
                    LoggingHelper.LogWarning("Non-success result when sending email from {Sender} to {Recipient}. Subject: {Subject}; Error: {Message}",
                        message.From?.EmailAddress?.Address, message.ToRecipients?.First()?.EmailAddress?.Address, message.Subject, result);
                    response.WarningMessage = result.ToString();
                    if (updateDetails)
                    {
                        MarkEmailAsFailed(request.EmailMessage, request.FailureStatus, result.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error while or before sending email from {Sender} to {Recipient}. Subject: {Subject}; Error: {Message}",
                    request.EmailMessage.From, request.EmailMessage.RecipientEmail, request.EmailMessage.Subject, ex.Message);
                response.WarningMessage = "An error occurred while attempting to send an email.";
                if (updateDetails)
                {
                    MarkEmailAsFailed(request.EmailMessage, request.FailureStatus, ex.Message);
                }
            }

            return response;
        }

        private Message MapEmailMessage(EmailMessageDto message)
        {
            var emailMessage = new Message
            {
                Subject = message.Subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = message.Body
                },
                ToRecipients = new List<Recipient>
                {
                    new Recipient {  EmailAddress = new EmailAddress(){ Address = message.RecipientEmail?.Trim()} }
                },

                From = new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = message.From?.Trim()
                    }
                }
            };
            if (!string.IsNullOrWhiteSpace(message.Cc))
            {
                var splitCc = message.Cc.Split(';');
                emailMessage.CcRecipients = splitCc
                    .Select(x => new Recipient { EmailAddress = new EmailAddress() { Address = x } })
                    .ToList();

            }

            if (!string.IsNullOrWhiteSpace(message.Bcc))
            {
                var splitBcc = message.Bcc.Split(';');
                emailMessage.BccRecipients = splitBcc
                    .Select(x => new Recipient { EmailAddress = new EmailAddress() { Address = x } })
                    .ToList();
            }

            if (message.Attachments != null)
            {
                emailMessage.Attachments = new MessageAttachmentsCollectionPage();
                var basePath = ConfigurationManager.AppSettings["fileSystemFileStorageServiceBasePath"];

                foreach (var item in message.Attachments)
                {
                    var storedFile = NHibernateSession.Current.Get<StoredFile>(item.StoredFileId);

                    if (storedFile != null)
                    {
                        byte[] contentBytes = System.IO.File.ReadAllBytes(Path.Combine(basePath, storedFile.ExternalFileId));
                        string contentType = "pdf";
                        var attachment = new FileAttachment
                        {

                            ODataType = "#microsoft.graph.fileAttachment",
                            ContentBytes = contentBytes,
                            ContentType = contentType,
                            ContentId = storedFile.FileName,
                            Name = item.FileName
                        };

                        emailMessage.Attachments.Add(attachment);
                    }
                }

            }

            return emailMessage;

        }

        public ServiceResponse SendAndForgetMail(GenericEmailMessageRequest request)
        {
            var useOffice365ToSendEmail =
                _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "UseOffice365ToSendEmail" }).Value;

            if (Convert.ToBoolean(useOffice365ToSendEmail))
            {
                return SendEmailUsingOffice365(request, false);
            }

            return SendMail(request, false);
        }

        private ServiceResponse SendMail(GenericEmailMessageRequest request, bool updateDetails)
        {
            var response = new ServiceResponse();

            try
            {
                var emailSenderAddress = NHibernateSession.Current.Query<SystemValue>()
                .First(x => x.ValueKey == "DefaultEmailSenderAddress");

                if (updateDetails)
                {
                    MarkEmailAsSending(request.EmailMessage);
                }

                var message = new MailMessage()
                {
                    From = new MailAddress(string.IsNullOrWhiteSpace(request.EmailMessage.From)
                        ? emailSenderAddress.Value?.Trim()
                        : request.EmailMessage.From?.Trim()),
                    IsBodyHtml = true,
                    Body = request.EmailMessage.Body,
                    Subject = request.EmailMessage.Subject
                };

                if (!string.IsNullOrWhiteSpace(request.EmailMessage.Bcc))
                {
                    var splitBcc = request.EmailMessage.Bcc.Split(';');
                    foreach (var item in splitBcc)
                        message.Bcc.Add(item);
                }

                if (!string.IsNullOrWhiteSpace(request.EmailMessage.Cc))
                {
                    var splitCc = request.EmailMessage.Cc.Split(';');
                    foreach (var item in splitCc)
                        message.Bcc.Add(item);
                }

                if (request.EmailMessage.Attachments != null)
                {
                    var basePath = ConfigurationManager.AppSettings["fileSystemFileStorageServiceBasePath"];

                    foreach (var item in request.EmailMessage.Attachments)
                    {
                        var storedFile = NHibernateSession.Current.Get<StoredFile>(item.StoredFileId);
                        if (storedFile != null)
                        {
                            var attachment = new Attachment(Path.Combine(basePath, storedFile.ExternalFileId));
                            // set name of attachment as storedFile.ExternalFileId includes unneeded text.
                            attachment.Name = storedFile.FileName;
                            message.Attachments.Add(attachment);
                        }
                    }
                }

                message.To.Add(request.EmailMessage.RecipientEmail?.Trim() ?? emailSenderAddress.Value?.Trim());

                using (var client = GetMailServer())
                {
                    client.Send(message);
                    if (updateDetails)
                    {
                        MarkEmailAsSuccessful(request.EmailMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                response.Error = true;
                response.ErrorMessage = ex.Message;
                response.StackTrace = ex.StackTrace;

                if (updateDetails)
                {
                    MarkEmailAsFailed(request.EmailMessage, request.FailureStatus, ex.Message);
                }
            }

            return response;
        }

        public EmailMessageListResponse GetEmailMessageByCredentialApplicationId(int credentialApplicationId)
        {
            var emailMessages =
                NHibernateSession.Current.Query<CredentialApplicationEmailMessage>()
                    .Where(x => x.CredentialApplication.Id == credentialApplicationId)
                    .Select(x => new EmailMessageDto
                    {
                        EmailMessageId = x.EmailMessage.Id,
                        CreatedDate = x.EmailMessage.CreatedDate,
                        Subject = x.EmailMessage.Subject,
                        RecipientEmail = x.EmailMessage.RecipientEmail,
                        LastSendAttemptDate = x.EmailMessage.LastSendAttemptDate,
                        LastSendResult = x.EmailMessage.LastSendResult,
                        EmailTemplateId = x.EmailMessage.EmailTemplate.Id
                    }).ToList();

            return emailMessages.Any()
                ? new EmailMessageListResponse { Data = emailMessages }
                : new EmailMessageListResponse();
        }

        public EmailMessageListResponse GetEmailMessageByMaterialRequestId(int materialRequestId)
        {
            var emailMessages =
                NHibernateSession.Current.Query<MaterialRequestEmailMessage>()
                    .Where(x => x.MaterialRequest.Id == materialRequestId)
                    .Select(x => new EmailMessageDto
                    {
                        EmailMessageId = x.EmailMessage.Id,
                        CreatedDate = x.EmailMessage.CreatedDate,
                        Subject = x.EmailMessage.Subject,
                        RecipientEmail = x.EmailMessage.RecipientEmail,
                        LastSendAttemptDate = x.EmailMessage.LastSendAttemptDate,
                        LastSendResult = x.EmailMessage.LastSendResult
                    }).ToList();

            return emailMessages.Any()
                ? new EmailMessageListResponse { Data = emailMessages }
                : new EmailMessageListResponse();
        }


        public ServiceResponse<IEnumerable<EmailMessageResultDto>> SearchEmail(GetEmailSearchRequest request)
        {

            if (!(request.SortingOptions?.Any() ?? false))
            {
                request.SortingOptions = new List<EmailMessageSortingOption>
                {
                    new EmailMessageSortingOption
                    {
                        SortType = EmailSortType.LastSendAttemptDate,
                        SortDirection = SortDirection.Descending
                    }
                };
            }
            var emailMessages = new EmailMessageQueryHelper().SearchEmails(request);
            return new ServiceResponse<IEnumerable<EmailMessageResultDto>>() { Data = emailMessages };

        }

        private SmtpClient GetMailServer()
        {
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            return smtpSection == null ? new SmtpClient() : new SmtpClient(smtpSection.Network.Host);
        }

        public EmailMessageAttachmentListResponse GetMailAttachment(string graphMailId, string emailAddress)
        {
            var response = new EmailMessageAttachmentListResponse();
            var authService = new AzureAuthorisationService(_systemQueryService, _secretsProvider);
            var o365 = new Office365IntegrationService(authService);

            var attachments = o365.GetEmailAttachment(graphMailId, emailAddress)
                .Select(x => new EmailMessageAttachmentDto()
                {
                    FileName = x.Name,
                    GraphAttachmentSize = x.Size,
                    GraphAttachmentBytes = x.ContentBytes,
                    FileType = x.ContentType,
                    IsInline = x.IsInline ?? false,
                    ContentId = x.ContentId
                });

            response.Data = attachments;
            return response;
        }

        public ServiceResponse<EmailMessageDto> GetMailDetails(string graphMailId, string emailAddress)
        {
            var response = new ServiceResponse<EmailMessageDto>();
            var authService = new AzureAuthorisationService(_systemQueryService, _secretsProvider);
            var o365 = new Office365IntegrationService(authService);

            var message = o365.GetEmailDetails(graphMailId, emailAddress);
            var email = new EmailMessageDto()
            {
                GraphEmailMessageId = message.Id,
                From = message.From.EmailAddress.Address,
                RecipientEmail = string.Join(";", message.ToRecipients.FirstOrDefault()?.EmailAddress.Address ?? string.Empty),
                Subject = message.Subject,
                LastSendAttemptDate = ConvertLocalTime(message.SentDateTime?.DateTime),
                HasAttachments = message.HasAttachments ?? false,
                Body = message.Body.Content,
                CreatedDate = ConvertLocalTime(message.CreatedDateTime?.DateTime),
            };

            response.Data = email;
            return response;

        }

        public EmailMessageHeaderListResponse GetMailMessagesFromSharedAccount(List<string> sharedAccount, string emailAddress)
        {
            var response = new EmailMessageHeaderListResponse();
            if (!sharedAccount.Any())
                return response;

            try
            {
                var authenticator = new AzureAuthorisationService(_systemQueryService, _secretsProvider);
                var o365 = new Office365IntegrationService(authenticator);

                var messagesDictionary = o365.GetMailMessagesFromSharedAccount(sharedAccount, emailAddress).ToList();

                var emails = messagesDictionary.SelectMany(d => d.Value.Select(x=>
                    new EmailMassageHeaderDto
                    {
                        GraphEmailMessageId = x.Id,
                        From = GetFromAddress(x.From.EmailAddress.Address),
                        Subject = x.Subject,
                        LastSendAttemptDate = ConvertLocalTime(x.SentDateTime?.DateTime),
                        HasAttachments = x.HasAttachments ?? false,
                        MailBox = d.Key
                    }));

                response.Data = emails.OrderByDescending(x => x.LastSendAttemptDate).ToList();
            }
            catch (WebServiceException ex)
            {
                response.Error = true;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        private string GetFromAddress(string address)
        {
            if (address.Contains("/cn="))
            {
                var splitString = address.Split(new[] {"/cn="},StringSplitOptions.None).ToList();
                if (splitString.Any())
                {
                    address = splitString.Last();
                }
            };
            return address;
        }

        private DateTime? ConvertLocalTime(DateTime? dateTime)
        {
            if (dateTime == null)
                return null;

            var localTime = TimeZone.CurrentTimeZone.ToLocalTime(dateTime.Value);

            return localTime;
        }

        public GetEmailTemplateResponse GetSystemEmailTemplate(GetSystemEmailTemplateRequest request)
        {
            var templates = new List<EmailTemplateDetailDto>();
            var response = new GetEmailTemplateResponse { Data = templates };

            var actions = request.Actions.Select(x => (int)x).ToArray();

            var query = NHibernateSession.Current
                .Query<SystemActionEmailTemplate>()
                .Where(x => actions.Contains(x.SystemActionType.Id) && x.Active);

            if (request.ActionEvents != null)
            {
                var eventsIds = request.ActionEvents?.Select(x => (int)x).ToList();
                query = query.Where(x => eventsIds.Contains(x.SystemActionEventType.Id));
            }

            var mappings = query.ToList();

            if (!mappings.Any())
            {
                var actionValues = string.Join(",",
                    request.ActionEvents ?? Enumerable.Empty<SystemActionEventTypeName>());
                response.Error = true;
                response.ErrorMessage = $"Could not find an email template for the Action combination of {string.Join(",", request.Actions)} and events {actionValues}.";
            }
            else
            {
                foreach (var template in mappings)
                {

                    var emailTemplate = _autoMapperHelper.Mapper.Map<EmailTemplateDetailDto>(template.EmailTemplate);
                    emailTemplate.EmailTemplateDetails = template.EmailTemplateDetails.Select(x => (EmailTemplateDetailTypeName)x.Id);
                    emailTemplate.SystemActionEventType = (SystemActionEventTypeName)template.SystemActionEventType.Id;
                    templates.Add(emailTemplate);
                }

            }
            return response;
        }

        public void UpdateEmailMessageSendStatus(int emailMessageId, EmailSendStatusTypeName status)
        {
            var emailMessage = NHibernateSession.Current.Get<EmailMessage>(emailMessageId);
            if (emailMessage != null && emailMessage.EmailSendStatusType?.Id != (int)status)
            {
                emailMessage.EmailSendStatusType = NHibernateSession.Current.Get<EmailSendStatusType>((int)status);
                NHibernateSession.Current.Save(emailMessage);
                NHibernateSession.Current.Flush();
            }
        }
    }
}
