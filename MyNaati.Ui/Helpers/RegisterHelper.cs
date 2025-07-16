using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MyNaati.Bl.BackOffice.Helpers;
using MyNaati.Contracts.BackOffice.Common;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Contracts.Portal.Users;
using MyNaati.Ui.Controllers;
using MyNaati.Ui.Models;

namespace MyNaati.Ui.Helpers
{
    public class RegisterHelper : IRegisterHelper
    {
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly IMembershipProviderService mMembershipService;
        private readonly IUserService mUserService;
        public static string GENERIC_USER_REGISTRATION_REQUEST_ERROR = "A problem occurred while registering you for myNAATI. Please contact NAATI.";
        private readonly IEmailService mEmailService;
        private readonly IFormsAuthenticationService mFormsService;
        public RegisterHelper(IPersonalDetailsService personalDetailsService,
            IMembershipProviderService membershipService,
            IUserService userService,
            IEmailService emailService,
            IFormsAuthenticationService formsService)
        {
            mPersonalDetailsService = personalDetailsService;
            mMembershipService = membershipService;
            mUserService = userService;
            mEmailService = emailService;
            mFormsService = formsService;
        }

        public void ValidateRegistrationInputs(IRegisterModel model, ModelStateDictionary modelState)
        {
            //const int DEBTOR_NAATI_NUMBER = 910000;
            int naatiNumber;

            if (!int.TryParse(model.NaatiNumber, out naatiNumber) || naatiNumber <= 0)
            {
                modelState.AddModelError("NaatiNumber", "Customer number must contain only numbers and must be above zero.");
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                modelState.AddModelError("Email", "Email field mandatory");
            }
        }

        /// <summary>
        /// Creates an asp.net user in the ePortal database and updates the SAM database to say the Person is
        /// now registered in the ePortal and sends them an email with their registration details.  It may 
        /// optionally update or add the Person's email address.
        /// </summary>
        /// <param name="primaryEmail">The email address of the person being registered.</param>
        /// <returns></returns>
        public CreateUserResponse CreateUser(int naatiNumber, string primaryEmail)
        {
            //  Get the persons name for the email.
            var request = new PersonNaatiNumberRequest() { NaatiNumber = naatiNumber };
            PersonalDetailsGetPersonResponse personResponse = mPersonalDetailsService.GetPerson(request);

            // Attempt to register the user, a null password will generate a password for the user.
            MembershipCreateResult createResult = mMembershipService.CreateUser(primaryEmail,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                true,
                                                                                null,
                                                                                false,
                                                                                personResponse.Person.GivenName, personResponse.Person.Surname, false, personResponse.Person.NaatiNumber);

            return new CreateUserResponse { CreateStatus = createResult.CreateStatus, EmailSuccess = createResult.EmailSuccess };
        }

        public void RegisterEmailChange(string currentPrimaryEmail, string newEmail, string emailUrl, int emailChangeValidHours)
        {
            var expiryDate = DateTime.Now.AddHours(emailChangeValidHours);
            var user = mMembershipService.GetUser(currentPrimaryEmail, false);
            var naatiNumber = mUserService.GetUser(user.UserId).NaatiNumber;
            var emailChangeResponse = mUserService.RegisterEmailChange(new EmailChangeRequest { CurrentPrimaryEmail = currentPrimaryEmail, NewEmail = newEmail, UserId = user.UserId, ExpiryDate = expiryDate });

            var personResponse =
                mPersonalDetailsService.GetPerson(new PersonNaatiNumberRequest {NaatiNumber = naatiNumber});

            //  Send an email to the user advising of their new password.
            var emailRequest = new SendEmailRequest(EmailTemplate.EmailChangeRequest, currentPrimaryEmail);
            emailRequest.Tokens.Add(EmailTokens.GivenName, personResponse.Person.GivenName);
            emailRequest.Tokens.Add(EmailTokens.Email, newEmail);
            emailRequest.Tokens.Add(EmailTokens.Hours, emailChangeValidHours.ToString());
            emailRequest.Tokens.Add(EmailTokens.NaatiNumber, personResponse.Person.NaatiNumber.ToString());
            emailRequest.Tokens.Add(EmailTokens.Url, emailUrl.Replace($"{nameof(RegisterEmailChange)}", emailChangeResponse.Reference.ToString()));
            mEmailService.SendMail(emailRequest);
        }

        public bool HasPendingPrimaryEmailChange(string primaryEmail, int emailChangeValidHours)
        {
            var expiryDate = DateTime.Now.AddHours(emailChangeValidHours * -1);
            var user = mMembershipService.GetUser(primaryEmail, false);
            var result = mUserService.GetRegisteredEmailChangeByUser(user.UserId);
            return result.Items.Any(x => x.Expiry >= expiryDate);
        }

        public GetRegisteredEmailChangeResponse GetRegisteredEmailChange(int reference, int emailChangeValidHours)
        {
            var expiryDate = DateTime.Now.AddHours(emailChangeValidHours * -1);
            var response = new GetRegisteredEmailChangeResponse();
            var emailChangeItems =
                mUserService.GetRegisteredEmailChange(new GetEmailChangeRequest { Reference = reference, ExpiryDate = expiryDate });
            if (!emailChangeItems.Items.Any())
            {
                response.ErrorMessage = "It seems your request already expired. Please submit a new request.";
                return response;
            }

            if (emailChangeItems.Items.Count() > 1)
            {
                response.ErrorMessage = "A general error occurred. Please submit a new request.";
                return response;
            }

            response.PrimaryEmail = emailChangeItems.Items.ElementAt(0).PrimaryEmail;
            response.SecondaryEmail = emailChangeItems.Items.ElementAt(0).SecondaryEmail;
            return response;
        }
        public ChangeLogonEmailResponse ChangeLogOnEmail(int reference, string password, int emailChangeValidHours)
        {
            var expiryDate = DateTime.Now.AddHours(emailChangeValidHours * -1);
            mFormsService.SignOut();
            var emailChangeItems =
                mUserService.GetRegisteredEmailChange(new GetEmailChangeRequest { Reference = reference, ExpiryDate = expiryDate });

            if (!emailChangeItems.Items.Any())
            {
                return new ChangeLogonEmailResponse { ErrorMessage = "It seems your request already expired. Please submit a new request." };
            }

            if (emailChangeItems.Items.Count() > 1)
            {
                return new ChangeLogonEmailResponse { ErrorMessage = "A general error occurred. Please submit a new request." };
            }

            var emailChangeRequest = emailChangeItems.Items.ElementAt(0);

            var userIsValid = mMembershipService.ValidateUser(emailChangeRequest.PrimaryEmail, password);
            if (!userIsValid)
            {
                return new ChangeLogonEmailResponse { ErrorMessage = "Validation was not successful. Please check your password" };
            }

            var naatiNumber =
                mUserService.GetUser(mMembershipService.GetUser(emailChangeRequest.PrimaryEmail, true).UserId)
                    .NaatiNumber;

            var personRequest = new PersonNaatiNumberRequest { NaatiNumber = naatiNumber };
            var emails = mPersonalDetailsService.GetEmails(personRequest);

            var user = mMembershipService.GetUser(emailChangeRequest.SecondaryEmail, false);
            if (user != null)
            {
                return new ChangeLogonEmailResponse { ErrorMessage = $"Email {emailChangeRequest.SecondaryEmail} is already being used by other user." };
            }

            var emailToUpdate = emails.Emails.FirstOrDefault(x => EmailsMatch(x.Email, emailChangeRequest.SecondaryEmail));
            if (emailToUpdate == null)
            {
                emailToUpdate = emails.Emails.First(x => x.IsPreferred);
                emailToUpdate.Email = emailChangeRequest.SecondaryEmail;
            }

            var result = mPersonalDetailsService.UpdateEmail(new PersonalDetailsUpdateEmailRequest
            {
                AllowChangePrimary = true,
                NaatiNumber = naatiNumber,
                UserName = emailChangeRequest.PrimaryEmail,
                Email =  new PersonalEditEmail { Email = emailToUpdate.Email, EmailId = emailToUpdate.EmailId, IsPreferred = true }
            });

            if (!result.WasSuccessful)
            {
                return new ChangeLogonEmailResponse { ErrorMessage = result.ErrorMessage };
            }

            mUserService.ChangeUserName(new ChangeUserNameRequest { EmailChangeId = emailChangeRequest.Id });

            mMembershipService.UpdateUserEmailAddressIfPresent(emailChangeRequest.SecondaryEmail,
                emailChangeRequest.SecondaryEmail);

            mFormsService.SignIn(emailToUpdate.Email, false);

            personRequest = new PersonNaatiNumberRequest { NaatiNumber = naatiNumber };
            var personResponse = mPersonalDetailsService.GetPerson(personRequest);

            //  Send an email to the user advising of their new password.
            var emailRequest = new SendEmailRequest(EmailTemplate.EmailChangeConfirmation, emailToUpdate.Email);
            emailRequest.Tokens.Add(EmailTokens.GivenName, personResponse.Person.GivenName);
            emailRequest.Tokens.Add(EmailTokens.NaatiNumber, personResponse.Person.NaatiNumber.ToString());
            mEmailService.SendMail(emailRequest);

            return new ChangeLogonEmailResponse { NewLogonEmail = emailToUpdate.Email };
        }

        public bool EmailsMatch(string first, string second)
        {
            first = first.Trim();
            second = second.Trim();
            return first.Equals(second, StringComparison.InvariantCultureIgnoreCase);
        }

        public string ValidateRegistrationRequest(string email, int naatiNumber)
        {
            var personRequest = new PersonNaatiNumberRequest {NaatiNumber = naatiNumber, };
            var emailValidationresponse =
                mPersonalDetailsService.ValidatePrimaryEmail(email);

            if (!string.IsNullOrWhiteSpace(emailValidationresponse.ErrorMessage))
            {
                return emailValidationresponse.ErrorMessage;
            }

            if (emailValidationresponse.NaatiNumber != naatiNumber)
            {
                return "Email and/or Customer Number you provided doesnt match with our database.";
            }

            var personResponse = mPersonalDetailsService.GetPerson(personRequest);

            if (personResponse?.Person == null || personResponse.Person.Deceased)
            {
                return "Person was not found or is not able to create a myNAATI account";
            }

            return null;

        }

        public void ValidateCaptcha(bool captchaValid, string captchaErrorMessage, ModelStateDictionary modelState)
        {
            if (!captchaValid)
            {
                modelState.AddModelError("Captcha", GetCaptchaMessage(captchaValid, captchaErrorMessage));
            }
        }

        public static string GetCaptchaMessage(bool captchaValid, string captchaErrorMessage)
        {
            if (!captchaValid)
            {
                if (!string.IsNullOrEmpty(captchaErrorMessage))
                {
                    switch (captchaErrorMessage.Trim())
                    {
                        case "Invalid reCAPTCHA request. Missing response value.":
                        case "The verification words are incorrect.":
                            //  Ignore this and add the default message.
                            break;
                        default:
                            //  Unexpected error so display it in full.
                            return captchaErrorMessage;
                    }
                }

                return "The words you have entered do not match the words in the image.";
            }

            return null;
        }

        private string EnforceNamingConvention(string name)
        {
            // Add a new function that takes the name (first or family), parses each word, if word matches ignore, otherwise
            // make it Title case e.g. Chris instead of CHRIS, 
            var excludedNameList = ApplicationSettingsHelper.ExcludedNamesList;
            var names = name.ToLower().Split(' ');

            var fixedName = new StringBuilder();

            foreach (string n in names)
            {
                if (n.Contains('-'))
                {
                    bool hyphenReInserted = false;

                    foreach (var n1 in n.Split('-'))
                    {
                        fixedName.Append(excludedNameList.All(x => x.ToLower() != n1.ToLower()) ? TextHelper.FirstLetterToUpper(n1) : n1);

                        if (!hyphenReInserted)
                        {
                            fixedName.Append("-");
                            hyphenReInserted = true;
                        }
                    }
                }
                else
                {
                    fixedName.Append(excludedNameList.All(x => x.ToLower() != n.ToLower()) ? TextHelper.FirstLetterToUpper(n) : n);
                    fixedName.Append(" ");
                }
            }

            return fixedName.ToString().Trim();
        }
    }
}