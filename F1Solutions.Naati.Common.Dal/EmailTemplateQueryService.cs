using System;
using System.Linq;
using System.Text.RegularExpressions;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using Resources = Naati.Resources;

namespace F1Solutions.Naati.Common.Dal
{

    public class EmailTemplateQueryService : IEmailTemplateQueryService
    {
        public EmailTemplateSearchResultResponse SearchEmailTemplates(EmailTemplateSearchRequest request)
        {
            var queryHelper = new EmailTemplateQueryHelper();
            return new EmailTemplateSearchResultResponse { Results = queryHelper.SearchEmailTemplates(request) };
        }

        public EmailTemplateResponse Get(EmailTemplateRequest model)
        {
            var emailTemplate = NHibernateSession.Current.Get<EmailTemplate>(model.Id);
            var workflows =
                NHibernateSession.Current.Query<SystemActionEmailTemplate>()
                    .Where(x => x.EmailTemplate.Id == model.Id).ToList()
                    .Select(x => new WorkflowResponse
                    {
                        ApplicationTypes = x.CredentialWorkflowActionEmailTemplates.Select(y => new LookupTypeDto() { Id = y.CredentialApplicationType.Id, DisplayName = y.CredentialApplicationType.DisplayName }),
                        WorkflowName = x.SystemActionType.Name,
                        WorkflowDisplayName = x.SystemActionType.DisplayName,
                        Id = x.SystemActionType.Id,
                        EmailTemplateDetails = x.EmailTemplateDetails.Select(d => new LookupTypeDto { Id = d.Id, DisplayName = d.DisplayName }).ToList(),
                        Active = x.Active,
                        EventType = new LookupTypeDto { Id = x.SystemActionEventType.Id, DisplayName = x.SystemActionEventType.DisplayName }
                    }).ToList();

            return new EmailTemplateResponse
            {
                Active = emailTemplate.Active,
                Content = emailTemplate.Content,
                FromAddress = emailTemplate.FromAddress,
                Name = emailTemplate.Name,
                Subject = emailTemplate.Subject,
                WorkflowModels = workflows,
                Id = emailTemplate.Id
            };
        }

        public void Save(EmailTemplateRequest model)
        {
            var emailTemplate = NHibernateSession.Current.Get<EmailTemplate>(model.Id);

            if (emailTemplate == null)
                throw new ArgumentNullException(nameof(emailTemplate));

            emailTemplate.Name = model.Name ?? emailTemplate.Name;
            //emailTemplate.Content = PreserveNonBodyHtml(emailTemplate.Content, model.Content);
            emailTemplate.Content = BuildEmailContent(model.Content);
            emailTemplate.FromAddress = model.FromAddress ?? emailTemplate.FromAddress;
            emailTemplate.Subject = model.Subject ?? emailTemplate.Subject;
            emailTemplate.ModifiedByNaati = true;
            emailTemplate.ModifiedDate = DateTime.Now;
            emailTemplate.ModifiedUser = model.UserId;

            NHibernateSession.Current.Save(emailTemplate);
            NHibernateSession.Current.Flush();
        }


        private string BuildEmailContent(string newContent)
        {
            //get newContent
            const string newContentExpr = @"^.+<body>(.*)<//body>.*$";
            var regEx = new Regex(newContentExpr, RegexOptions.Singleline);

            var matchNewContent = regEx.Match(newContent);
            if (matchNewContent.Success)
            {
                //if a match then just take that bit
                newContent = matchNewContent.Groups[1].Value;
            }

            var contentHeader = Resources.EmailMessage.EmailHeader;
            // Footer is being tokenized
            //var contentFooter = Resources.EmailMessage.EmailFooter;

            var returnedHtml = $"{newContent}";
            returnedHtml = PutInLineBreaksAndImageCorrection(returnedHtml);

            returnedHtml = $"{contentHeader}{returnedHtml}";

            return returnedHtml;
        }
        private string PreserveNonBodyHtml(string currentContent, string newContent)
        {
            //remove current body
            const string currentContentExpr = @"^(.+<body>).*(</body>.*)$";
            var regEx = new Regex(currentContentExpr, RegexOptions.Singleline);

            var matchCurrentContent = regEx.Match(currentContent);
            if (!matchCurrentContent.Success)
            {
                //if for some reason we dont match then return the new content
                return newContent;
            }

            //get newContent
            const string newContentExpr = @"^.+<body>(.*)<//body>.*$";
            regEx = new Regex(newContentExpr, RegexOptions.Singleline);

            var matchNewContent = regEx.Match(newContent);
            if (matchNewContent.Success)
            {
                //if a match then just take that bit
                newContent = matchNewContent.Groups[1].Value;
            }
            var contentPrefix = matchCurrentContent.Groups[1].Value;
            var contentSuffix = matchCurrentContent.Groups[2].Value;

            var returnedHtml = $"{contentPrefix}{newContent}{contentSuffix}";
            returnedHtml = PutInLineBreaksAndImageCorrection(returnedHtml);
            return returnedHtml;
        }

        private string PutInLineBreaksAndImageCorrection(string returnedHtml)
        {
            // We do not need this anymore!
            //CKEditor removes this stuff.
            //returnedHtml = returnedHtml.Replace(@"Primary_logo_RGB.jpg"">", @"Primary_logo_RGB.jpg"" height=""80"" width=""80"" /> ");


            returnedHtml = returnedHtml.Replace("</p>", "</p>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("</tr>", "</tr>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("</td>", "</td>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("</tbody>", "</tbody>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("</body>", "</body>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("</table>", "</table>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("</ul>", "</ul>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("</li>", "</li>" + Environment.NewLine);

            //returnedHtml = returnedHtml.Replace("<p>", "<p>" + Environment.NewLine);
            //returnedHtml = returnedHtml.Replace("<tr>", "<tr>" + Environment.NewLine);
            //returnedHtml = returnedHtml.Replace("<td>", "<td>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("<tbody>", "<tbody>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("<body>", "<body>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("<table>", "<table>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("<ul>", "<ul>" + Environment.NewLine);
            returnedHtml = returnedHtml.Replace("<li>", "<li>" + Environment.NewLine);

            return returnedHtml;

        }
    }
}
