using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using Resources = Naati.Resources;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class TokenReplacementService : ITokenReplacementService
    {
        private static string GetTokenFieldValue(TokenReplacementField field,
            GetApplicationDetailsResponse applicationDetails,
            GetPersonDetailsResponse personDetails,
            GetContactDetailsResponse personContactDetails)
        {
            var personInfo = personDetails?.Results.FirstOrDefault();
            var personAddress = personContactDetails?.Addresses?.OrderByDescending(x => x.PrimaryContact).FirstOrDefault();

            switch (field)
            {
                case TokenReplacementField.TodayShort:
                    return DateTime.Now.ToShortDateString();
                case TokenReplacementField.TodayLong:
                    return DateTime.Now.ToLongDateString();
                case TokenReplacementField.CurrentTime:
                    return DateTime.Now.ToShortTimeString();
                case TokenReplacementField.NaatiNo:
                case TokenReplacementField.NaatiNumber:
                    return personInfo?.NaatiNumber.ToString();
                case TokenReplacementField.PractitionerNo:
                case TokenReplacementField.PractitionerNumber:
                    return personInfo?.PractitionerNumber;
                case TokenReplacementField.FamilyName:
                    return personInfo?.Surname;
                case TokenReplacementField.PrimaryEmail:
                    return personContactDetails?.Emails.FirstOrDefault(x => x.IsPreferredEmail)?.Email;
                case TokenReplacementField.GivenName:
                    return personInfo?.GivenName;
                case TokenReplacementField.OtherNames:
                    return personInfo?.OtherNames;
                case TokenReplacementField.PrimaryPhone:
                    return applicationDetails.ApplicantDetails.PrimaryContactNumber;
                case TokenReplacementField.PrimaryAddress:
                    return applicationDetails.ApplicantDetails.AddressLine1;
                case TokenReplacementField.PreferredTestLocation:
                    return "";
                case TokenReplacementField.Sponsor:
                    return applicationDetails.ApplicationInfo.SponsorInstitutionName;
                case TokenReplacementField.ApplicationType:
                    return applicationDetails.ApplicationType.DisplayName;
                case TokenReplacementField.ApplicationStatus:
                    return applicationDetails.ApplicationStatus.DisplayName;
                case TokenReplacementField.ApplicationReference:
                    return applicationDetails.ApplicationInfo.ApplicationReference;
                case TokenReplacementField.CredentialRequestStatus:
                    return applicationDetails.CredentialRequests.First().Status;
                case TokenReplacementField.CredentialType:
                    return InvalidTokenException(nameof(TokenReplacementField.CredentialType));
                case TokenReplacementField.Skill:
                    return InvalidTokenException(nameof(TokenReplacementField.Skill));
                case TokenReplacementField.ActionPublicNote:
                    return "";
                case TokenReplacementField.CertificateLongDate:
                    return InvalidTokenException(nameof(TokenReplacementField.CertificateLongDate));
                case TokenReplacementField.CertificateLongExpiryDate:
                    return InvalidTokenException(nameof(TokenReplacementField.CertificateLongExpiryDate));
                case TokenReplacementField.CertificateShortDate:
                    return InvalidTokenException(nameof(TokenReplacementField.CertificateShortDate));
                case TokenReplacementField.CertificateShortExpiryDate:
                    return InvalidTokenException(nameof(TokenReplacementField.CertificateShortExpiryDate));
                case TokenReplacementField.CredentialRequests:
                    return InvalidTokenException(nameof(TokenReplacementField.CredentialRequests));
                case TokenReplacementField.Country:
                    return personAddress?.CountryName;
                case TokenReplacementField.PostCode:
                    return personAddress?.Postcode;
                case TokenReplacementField.Suburb:
                    return personAddress?.SuburbName;
                case TokenReplacementField.State:
                    return personAddress?.StateAbbreviation;
                case TokenReplacementField.PaymentReference:
                    return InvalidTokenException(nameof(TokenReplacementField.PaymentReference));
                case TokenReplacementField.EmailFooter:
                    return Resources.EmailMessage.EmailFooter;
                case TokenReplacementField.Div:
                    return Resources.EmailMessage.Div;
                case TokenReplacementField.DivEnd:
                    return Resources.EmailMessage.DivEnd;
                default:
                    return "";
            }
        }

        private static string InvalidTokenException(string tokenName)
        {
            throw new Exception($"{tokenName} value has to be specified in the extratoken");
        }

        // todo: make an overload that takes the credential application details, to save another server call when possible
        public string ReplaceTemplateFieldValues(string content,
            GetApplicationDetailsResponse applicationDetails,
            GetPersonDetailsResponse personDetails,
            GetContactDetailsResponse personContactDetails,
            IDictionary<string, string> extraTokens,
            bool logExceptions,
            out IEnumerable<string> errors)
        {
            var valueToReplace = content;
            Action<string, string> defaultReplacementAction = (token, value) => valueToReplace = valueToReplace.Replace(token, value ?? string.Empty);

            ReplaceValues(content,
                applicationDetails,
                personDetails,
                personContactDetails,
                defaultReplacementAction,
                extraTokens,
                logExceptions,
                out errors);

            return valueToReplace;
        }

        public string GetTokenNameFor(TokenReplacementField token)
        {
            return typeof(TokenReplacementField)
                .GetMember(token.ToString())
                .First().GetCustomAttribute<DisplayAttribute>()
                .Name;
        }

        public void ReplaceTemplateFieldValues(string content,
            GetApplicationDetailsResponse applicationDetails,
            GetPersonDetailsResponse personDetails,
            GetContactDetailsResponse personContactDetails,
            Action<string, string> replacementAction,
            IDictionary<string, string> extraTokens,
            bool logExceptions,
            out IEnumerable<string> errors)
        {
            ReplaceValues(content, applicationDetails, personDetails, personContactDetails, replacementAction, extraTokens, logExceptions, out errors);
        }

        private void ReplaceValues(string content,
            GetApplicationDetailsResponse applicationDetails,
            GetPersonDetailsResponse personDetails,
            GetContactDetailsResponse personContactDetails,
            Action<string, string> replacementAction,
            IDictionary<string, string> extraTokens,
            bool logExceptions,
            out IEnumerable<string> errors)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(content))
            {
                errorMessages.Add($"{nameof(content)} is null");
                errors = errorMessages;
                return;
            }

            var listTokensInContent =
                Regex.Matches(content.Replace(Environment.NewLine, ""), @"\[\[([^]]*)\]\]")
                    .Cast<Match>()
                    .Select(x => x.Groups[1].Value)
                    .ToList();

            var listTokens =
                System.Enum.GetNames(typeof(TokenReplacementField))
                    .Select(
                        x =>
                            ((TokenReplacementField)System.Enum.Parse(typeof(TokenReplacementField), x)).GetType()
                            .GetMember(x)
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .Name).ToList();

            foreach (var field in listTokensInContent)
            {
                var index = listTokens.FindIndex(x => string.Compare(x, field, StringComparison.OrdinalIgnoreCase) == 0);
                if (index != -1)
                {
                    var token = $"[[{field}]]";

                    if (extraTokens != null && extraTokens.ContainsKey(field))
                    {
                        replacementAction(token, extraTokens[field]);
                    }
                    else
                    {
                        var value =
                            GetTokenFieldValue(
                                (TokenReplacementField)System.Enum.Parse(typeof(TokenReplacementField), index.ToString()),
                                applicationDetails, personDetails, personContactDetails);

                        var notStatedValue = "[NOT STATED]";
                        var replaceTokenValue = (string.IsNullOrEmpty(value) || string.Equals(value.ToUpper(), notStatedValue)) ? string.Empty : value;
                        replacementAction(token, replaceTokenValue);
                    }

                }
                else
                {
                    var error = $"Token: [[{field}]] not found.";
                    errorMessages.Add(error);
                    if (logExceptions)
                    {
                        var hex = new HandledException(error, $"Error replacing tokens for content: {content}", "NCMS");
                        LoggingHelper.LogException(hex, "Error replacing tokens for content: {Content}: {Message}", content, error);
                    }
                }
            }

            errors = errorMessages;
        }

        public void ReplaceTemplateFieldValues(string content, Action<string, string> replacementAction, IDictionary<string, string> extraTokens, bool logExceptions,
            out IEnumerable<string> errors)
        {
            ReplaceTestSessionValues(content, replacementAction, extraTokens, logExceptions, out errors);
        }

        private void ReplaceTestSessionValues(string content,
           Action<string, string> replacementAction,
           IDictionary<string, string> extraTokens,
           bool logExceptions,
           out IEnumerable<string> errors)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(content))
            {
                errorMessages.Add($"{nameof(content)} is null");
                errors = errorMessages;
                return;
            }

            var listTokensInContent =
                Regex.Matches(content.Replace(Environment.NewLine, ""), @"\[\[([^]]*)\]\]")
                    .Cast<Match>()
                    .Select(x => x.Groups[1].Value)
                    .ToList();

            var listTokens =
                System.Enum.GetNames(typeof(TokenReplacementField))
                    .Select(
                        x =>
                            ((TokenReplacementField)System.Enum.Parse(typeof(TokenReplacementField), x)).GetType()
                            .GetMember(x)
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .Name).ToList();

            foreach (var field in listTokensInContent)
            {
                var index = listTokens.FindIndex(x => string.Compare(x, field, StringComparison.OrdinalIgnoreCase) == 0);
                if (index != -1)
                {
                    var token = $"[[{field}]]";
                    if (extraTokens.ContainsKey(field))
                    {
                        replacementAction(token, extraTokens[field]);
                    }
                }
                else
                {
                    var error = $"Token: [[{field}]] not found.";
                    errorMessages.Add(error);
                    if (logExceptions)
                    {
                        var hex = new HandledException(error, $"Error replacing tokens for content: {content}", "NCMS");
                        LoggingHelper.LogException(hex, "Error replacing tokens for content: {Content}: {Message}", content, error);
                    }
                }
            }

            errors = errorMessages;
        }

        public int TotalTokens(string content)
        {
            return Regex.Matches(content.Replace(Environment.NewLine, ""), @"\[\[([^]]*)\]\]")
                .Cast<Match>()
                .Select(x => x.Groups[1].Value)
                .Count();
        }

    }
}
