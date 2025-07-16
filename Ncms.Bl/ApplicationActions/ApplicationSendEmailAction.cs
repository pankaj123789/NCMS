using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationSendEmailAction : ApplicationStateAction 
    {
        protected override bool OverrideTemplates(IList<EmailTemplateModel> templates)
        {
            templates.Requires(x => x.Count == 1, $"The {nameof(SystemActionTypeName.SendEmail)} action must have exactly one email template.");

            // override template subject content with the values from the compose email step of the wizard
            var sendEmailStep = WizardModel.Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.ComposeEmail);
            if (sendEmailStep?.Data != null && (sendEmailStep.Data as Newtonsoft.Json.Linq.JObject).HasValues)
            {
                var template = templates.Single();
                template.Content = System.Web.HttpUtility.HtmlDecode(sendEmailStep.Data.body?.Value);
                template.Subject = sendEmailStep.Data.subject?.Value;
                return true;
            }

            return false;
        }
    }
}
