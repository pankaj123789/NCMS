using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.SystemActions;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.TestSessionActions
{
    public class TestSessionStateAction : SystemAction<SystemActionModel, SystemActionWizardModel, TestMaterialActionOutput, UpsertTestSessionResultModel, EmailMessageModel>
    {
        protected ITestSessionQueryService TestSessionQueryService => ServiceLocatorInstance.Resolve<ITestSessionQueryService>();
        protected ITestSpecificationQueryService TestSpecificationQueryService => ServiceLocatorInstance.Resolve<ITestSpecificationQueryService>();
        
        public TestSessionStateAction(IServiceLocator serviceLocator = null) : base(serviceLocator)
        {

        }

        protected override EmailMessageModel CreateEmail(EmailTemplateModel template, IDictionary<string, string> tokenDictionary)
        {

            IEnumerable<string> contenterrors;
            var content = TokenReplacementService.ReplaceTemplateFieldValues(template.Content, null, null, null, tokenDictionary, true,
                out contenterrors);

            var subject = template.Subject;

            var fromAddress = string.IsNullOrWhiteSpace(template.FromAddress)
                ? SystemService.GetSystemValue("DefaultEmailSenderAddress")
                : template.FromAddress;

            var email = new EmailMessageModel
            {
                Subject = subject,
                Body = content,
                From = fromAddress,
                CreatedDate = DateTime.Now,
                CreatedUserId = CurrentUser.Id,
                Attachments = new List<EmailMessageAttachmentModel>(),
                EmailSendStatusTypeId = (int)EmailSendStatusTypeName.Requested,
                EmailTemplateId = template.Id
            };

            return email;
        }

        protected override void CreateEmailAttachmentsIfApplicable()
        {
            return;
        }

        protected override IEnumerable<EmailMessageModel> GetEmails(EmailTemplateModel template, EmailMessageModel baseEmail)
        {
            throw new NotImplementedException();
        }

        public override IList<EmailTemplateModel> GetEmailTemplates()
        {
            if (!CanSendEmail())
            {
                return new List<EmailTemplateModel>();
            }

            return new List<EmailTemplateModel>();

        }

        protected override GenericResponse<UpsertTestSessionResultModel> SaveActionData()
        {
            throw new NotImplementedException();
        }

        protected override GenericResponse<EmailMessageModel> SaveEmailMessage(EmailMessageModel message)
        {
            throw new NotImplementedException();
        }
    }
}
