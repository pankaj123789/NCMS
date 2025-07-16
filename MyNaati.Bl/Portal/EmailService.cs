using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmailService" in both code and config file together.
    public class EmailService : IEmailService
    {
        private readonly IConfigurationRepository mConfigurationRepository;
        private readonly IEmailMessageQueryService mEmailMessageQueryService;

        public EmailService(IConfigurationRepository configurationRepository, IEmailMessageQueryService emailMessageQueryService)
        {
            mConfigurationRepository = configurationRepository;
            mEmailMessageQueryService = emailMessageQueryService;
        }
        #region IEmailService Members

        public SendEmailResponse SendMail(SendEmailRequest request)
        {
            var response = new SendEmailResponse { Success = false };

            try
            {
                var mails = PrepareEmail(request.TemplateKey, request.Recipients, request.Tokens);
                var errors = new List<string>();
                foreach (var mailMessage in mails)
                {
                    var emailResponse = mEmailMessageQueryService.SendAndForgetMail(new GenericEmailMessageRequest { EmailMessage = mailMessage, FailureStatus = EmailSendStatusTypeName.Failed });
                    if (emailResponse.Error)
                    {
                        errors.Add(emailResponse.ErrorMessage);
                    }
                }

                response.Success = !errors.Any();
                if (!response.Success)
                {
                    response.Exception = String.Join("\n", errors);
                    LoggingHelper.LogException(new Exception(response.Exception), "Error sending email.");
                }
            }
            catch (Exception e)
            {
                response.Exception = e.ToString();
                LoggingHelper.LogException(e, "Error sending email.");
            }

            return response;
        }

        #endregion

        private IEnumerable<EmailMessageDto> PrepareEmail(EmailTemplate templateKey, IEnumerable<string> recipients, IDictionary<EmailTokens, string> tokens)
        {
            var emails = new List<EmailMessageDto>();
            foreach (var recipient in recipients)
            {
                var email = new EmailMessageDto();
                email.From = GetFromEmail();
                email.RecipientEmail = recipient;
                email.Subject = GetSubject(templateKey);
                email.Body = TokeniseEmail(GetTemplate(templateKey), tokens);
                emails.Add(email);
            }
            return emails;
        }

        private string GetFromEmail()
        {
            return mConfigurationRepository.GetSystemValueBykey("EmailFromAddress").Value;
        }

        private static string GetSubject(EmailTemplate emailTemplate)
        {
            switch (emailTemplate)
            {
                case EmailTemplate.ApplicationSubmission:
                    return "myNAATI Application Submission";
                case EmailTemplate.EoiSubmission:
                    return "myNAATI EOI Submission";
                case EmailTemplate.NonCandidateRegisterSuccess:
                    return "myNAATI Registration";
                case EmailTemplate.RegisterSuccess:
                    return "myNAATI Registration";
                case EmailTemplate.ResetPassword:
                    return "myNAATI Password Reset";
                case EmailTemplate.NewPersonCreated:
                    return "You have been given a NAATI Customer Number";
                case EmailTemplate.EmailChangeRequest:
                    return "myNAATI: Email Change Verification";
                case EmailTemplate.EmailChangeConfirmation:
                    return "myNAATI: Email Change Confirmation";
                case EmailTemplate.SendAccessCode:
                    return "myNAATI: Access Code";
                default:
                    return string.Format("No subject defined for template '{0}'.", emailTemplate);
            }
        }

        private static string GetTemplate(EmailTemplate templateKey)
        {
            string fileName = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, string.Format(@"EmailTemplates\{0}.txt", templateKey));
            string template;

            using (var rdr = File.OpenText(fileName))
            {
                template = rdr.ReadToEnd();
            }

            return template;
        }

        private static string TokeniseEmail(string content, IDictionary<EmailTokens, string> tokens)
        {
            const string TOKEN_FORMAT = "<<{0}>>";
            string output = content;

            foreach (var key in tokens.Keys)
            {
                output = output.Replace(string.Format(TOKEN_FORMAT, key), tokens[key]);
            }

            return output;
        }


    }
}