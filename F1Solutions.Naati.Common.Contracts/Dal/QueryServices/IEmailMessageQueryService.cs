using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IEmailMessageQueryService : IQueryService
    {
        
        void UpdateEmailMessageDetails(EmailMessageDto request);

        
        ServiceResponse SendMail(GenericEmailMessageRequest email);

        
        ServiceResponse SendAndForgetMail(GenericEmailMessageRequest email);
     

        ServiceResponse SendMailById(int emailMessageId, EmailSendStatusTypeName failureStatus);

        
        EmailMessageResponse GetEmailMessage(int id);

        
        ApplicationEmailMessageResponse CreateApplicationEmailMessage(ApplicationEmailMessageRequest request);

        
        MaterialRequestEmailMessageResponse CreateMaterialRequestEmailMessage(MaterialRequestEmailMessageRequest request);

        
        EmailMessageResponse CreateGenericEmailMessage(GenericEmailMessageRequest request);

        
        EmailMessageListResponse GetEmailMessageByCredentialApplicationId(int credentialApplicationId);

        
        GetEmailMessageAttachmentsResponse GetAttachments(GetEmailMessageAttachmentsRequest request);

        
        EmailMessageHeaderListResponse GetMailMessagesFromSharedAccount(List<string> sharedAccount, string emailAddress);

        
        EmailMessageAttachmentListResponse GetMailAttachment(string graphMailId, string emailAddress);

        
        ServiceResponse<EmailMessageDto> GetMailDetails(string graphMailId, string emailAddress);

        
        GetEmailTemplateResponse GetSystemEmailTemplate(GetSystemEmailTemplateRequest request);

        
        ServiceResponse<IEnumerable<EmailMessageResultDto>> SearchEmail(GetEmailSearchRequest request);

        
        ServiceResponse ProcessPendingEmail(GenericEmailMessageRequest request);

        
        void UpdateEmailMessageSendStatus(int emailMessageId, EmailSendStatusTypeName status);

        
        EmailMessageListResponse GetEmailMessageByMaterialRequestId(int materialRequestId);
    }
}
