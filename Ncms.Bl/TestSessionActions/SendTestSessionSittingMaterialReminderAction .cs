using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;

namespace Ncms.Bl.TestSessionActions
{
    public class SendTestSessionSittingMaterialReminderAction : TestSessionStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Email;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Send;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateUserPermissions,
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            SendTestSessionSittingMaterialReminder
        };

        protected void SendTestSessionSittingMaterialReminder()
        {
            LoggingHelper.LogInfo($"Performing workflow action {SystemActionTypeName.SendTestSessionSittingMaterialReminder}");

            var testSessionIdsResult = TestSpecificationQueryService.GetTestSessionIdsWhereMaterialsNotYetFullyAllocated();
            if (!testSessionIdsResult.Success)
            {
                LoggingHelper.LogError(testSessionIdsResult.Errors.First(), "TestMaterialNotAllocatedReminder", "TestMaterialNotAllocatedReminder", false);
                throw new Exception(testSessionIdsResult.Errors.First());
            }

            var testSessionsToReport = testSessionIdsResult.Data;
            // if there are no test sessions to report then no need to send the email
            if(testSessionsToReport.Count == 0)
            {
                return;
            }

            // create list of combined ids and details before html formatting
            var testSessionContentBeforeFormatting = new List<string>();

            foreach (var testSessionId in testSessionsToReport)
            {
                var testSessionDetails = TestSessionQueryService.GetTestSessionDetailsById(testSessionId).Result;
                testSessionContentBeforeFormatting.Add(
                    // TS#ID# : dd/mm/yy HH:MMPM/AM for CredentialTypeInternalName 
                    $"TS{testSessionId} : {testSessionDetails.TestDate} for {testSessionDetails.CredentialTypeInternalName}"
                );
            }

            // add in the html formatting for the email template
            var testSessionsContentAfterFormatting = $"{(String.Join("</br><br>", testSessionContentBeforeFormatting))}</br>";

            //get template
            var emailTemplate = EmailTemplateQueryService.Get(new F1Solutions.Naati.Common.Contracts.Dal.Request.EmailTemplateRequest()
            {
                Id = 284
            });

            //replace the test session details token with our formatted content
            var content = emailTemplate.Content.Replace("[[Test Session Details]]", testSessionsContentAfterFormatting);

            var recipients = GetRecipients();
            foreach (var entry in recipients)
            {
                var response = EmailMessageService.CreateGenericEmailMessage(new EmailMessageModel()
                {
                    Body = content,
                    From = emailTemplate.FromAddress,
                    RecipientEmail = entry,
                    RecipientEntityId = 0,
                    Subject = emailTemplate.Subject,
                    CreatedUserId = CurrentUser.Id,
                    EmailSendStatusTypeId = (int)EmailSendStatusTypeName.Requested,
                    EmailTemplateId = emailTemplate.Id
                });
            }
        }

        protected override IEnumerable<BusinessServiceResponse> SendEmails()
        {
            var responses = new List<BusinessServiceResponse>();
            foreach (var emailModel in Output.PendingEmails)
            {
                var emailSendResponse = EmailMessageService.SendEmailMessageById(emailModel.EmailMessageId);

                responses.Add(emailSendResponse);
            }
            return responses;
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            //var availableRecertifyCredentials = ApplicationService.GetAvailableCredentialsToRecertify(ApplicationModel.ApplicationInfo.ApplicationId);

            //var skillTypes = ApplicationService.GetCredentialSkills(availableRecertifyCredentials).Data.ToList();
            //var eligibleRecertificationCredentials = string.Join(", ", skillTypes.Select(s => s.Externalname + "-" + s.DisplayName));

            //var certificationPeriod = ApplicationService.GetCertificationPeriod(ApplicationModel.ApplicationInfo.ApplicationId).Data;
            //var certificationPeriodHtml = certificationPeriod.EndDate.ToShortDateString();


            //tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.EligibleRecertificationCredentials), eligibleRecertificationCredentials);
            //tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CertificationPeriodEndDate), certificationPeriodHtml);

            base.GetEmailTokens(tokenDictionary);
        }

        protected override IEnumerable<EmailMessageModel> GetEmails(EmailTemplateModel template, EmailMessageModel baseEmail)
        {
            return base.GetEmails(template, baseEmail);
        }

        protected override bool CanSendEmail() => true;

        private List<string> GetRecipients()
        {
            var result = TestSessionQueryService.TestSittingsWithoutMaterialReminderEmailAddresses();
            if(result.Error)
            {
                throw new Exception("Failed to get email recipients (TestSittingsWithoutMaterialReminderEmailAddresses)");
            }
            return result.Data;
        }
    }
}
