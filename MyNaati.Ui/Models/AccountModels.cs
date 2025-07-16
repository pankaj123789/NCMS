using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.Common;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Ui.Attributes;
using MyNaati.Ui.Security;
using MyNaati.Ui.ViewModels.Shared;

namespace MyNaati.Ui.Models
{

    #region Models

    public class ChangePasswordModel
    {
        [DisplayName("Current password")]
        [Required]
        public string OldPassword { get; set; }

        [DisplayName("New password")]
        [Required]
        [MaxLength(40)]
        [RegularExpression(@"^.*(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).*$", ErrorMessage = "Passwords must contain a number, a lower case letter, a upper case letter and a symbol.")]
        public string NewPassword { get; set; }

        [DisplayName("Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The confirmed password does not match your new password.")]
        public string ConfirmPassword { get; set; }

    }

    public class LogOnModel
    {
        private string mUserName;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string UserName
        {
            get { return mUserName; }
            set { mUserName = value == null? string.Empty:value.Trim(); }
        }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool CredentialsRejected { get; set; }

        public bool CredentialLockedOut { get; set; }

        public string MessageOfTheDay { get; set; }

        public bool ShowMessageOfTheDay { get; set; }

        public bool MyNaatiAvailable { get; set; }

        public bool AccessDisabledByNaati { get; set; }

        public LogOnModel()
        {

        }
    }

    public interface IRegisterModel
    {
        string NaatiNumber { get; set; }

        string Email { get; set; }
    }

    public class ValidateExaminerModel
    {
        [Required]
        [Display(Name = "Security code")]
        public string SecurityCode { get; set; }

        public string Action { get; set; }
    }

    public class RegisterModel : IRegisterModel
    {
        public RegisterModel() { }

        public RegisterModel(string naatiNumber, string email, string confirmEmail)
        {
            NaatiNumber = naatiNumber;
            Email = email;
            ConfirmEmail = confirmEmail;
        }
     
        [DisplayName("Customer number")]
        [ValidateNaatiNumber]
        public string NaatiNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [DisplayName("Email")]
        [StringLength(200, ErrorMessage = "Email address must be no longer than 200 characters")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [System.ComponentModel.DataAnnotations.Compare("Email", ErrorMessage = "The confirmed email does not match the email you have entered.")]
        [DisplayName("Confirm email")]
        [UIHint("string")]
        public string ConfirmEmail { get; set; }
    }

    public class NonCandidateRegisterModel : IRegisterModel
    {
        public NonCandidateRegisterModel()
        {
        }


        public bool DetailsRequired { get; set; }

       
        [DisplayName("Customer number")]
        public string NaatiNumber { get; set; }

        [DisplayName("Title")]
        public int Title { get; set; }

        [Foolproof.RequiredIf(nameof(DetailsRequired), true, ErrorMessage = "The Given Name field is required.")]
        [StringLength(100)]
        [DisplayName("Given Name")]
        public string GivenName { get; set; }

        [StringLength(100)]
        [DisplayName("Middle Names")]
        public string MiddleNames { get; set; }
       
        [StringLength(100)]
        [DisplayName("Family name")]
        public string FamilyName { get; set; }
        
        [DisplayName("Gender")]
        public string Gender { get; set; }
        
        [Foolproof.RequiredIf(nameof(DetailsRequired), true, ErrorMessage = "Please select the data of birth.")]
        [DisplayName("Date of birth")]
        [LessThanOrEqualToCurrentDate]
        public DateTime DateOfBirth { get; set; }
        
        [DisplayName("Country of Birth")]
        [IsDifferentToValue(-1, ErrorMessage = "Please select a country of birth")]
        public int CountryOfBirth { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [DisplayName("Email")]
        [StringLength(200, ErrorMessage = "Email address must be no longer than 200 characters")]
        [UIHint("string")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [CaseInsensitiveCompare("Email", ErrorMessage = "The confirmed email does not match the email you have entered.")]
        [DisplayName("Confirm email")]
        [UIHint("string")]
        public string ConfirmEmail { get; set; }

        public IList<SelectListItem> TitleList { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> GenderList { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> Countries { get; set; } = new List<SelectListItem>();

        public string GmailWhiteListUrl { get; set; }
    }

    public class ManualRegisterModel : RegisterModel
    {
        public ManualRegisterModel() { }

        public ManualRegisterModel(RegisterModel registerModel)
        {
            NaatiNumber = registerModel.NaatiNumber;
            Email = registerModel.Email;
            ConfirmEmail = registerModel.ConfirmEmail;
        }

        [Required]
        [StringLength(100)]
        [DisplayName("Given name")]
        public string GivenName { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Family name")]
        public string FamilyName { get; set; }

        [Required]
        [DisplayName("Date of birth")]
        [LessThanOrEqualToCurrentDate(ErrorMessage = "Date of birth must be less than or equal to today's date")]
        public DateTime? DateOfBirth { get; set; }
    }

    public class ChangePrimaryEmailModel
    {
        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }

        public int Reference { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [DisplayName("Customer number")]
        [ValidateNaatiNumber]
        [UIHint("string")]
        public string NaatiNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [DisplayName("Email address")]
        [UIHint("string")]
        public string Email { get; set; }

        public string GmailWhiteListUrl { get; set; }
    }

    public class DuplicateUserResolutionModel
    {
        [Required]
        [DisplayName("Customer number")]
        public string NaatiNumber { get; set; }

        public IList<DuplicateUserModel> DuplicateUserList { get; set; }
    }

    public class DuplicateUserModel
    {
        [DisplayName("Customer number")]
        public string NaatiNumber { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Given name")]
        public string GivenName { get; set; }

        [DisplayName("Family name")]
        public string FamilyName { get; set; }

        [DisplayName("Date of birth")]
        [LessThanOrEqualToCurrentDate(ErrorMessage = "Date of birth must be less than or equal to today's date")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("ePortal Active")]
        public bool IsEportalActive { get; set; }
    }

    public class UserSearchModel
    {
        [DisplayName("Customer number")]
        [ValidateNaatiNumber]
        public string NaatiNumber { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Given name")]
        public string GivenName { get; set; }

        [DisplayName("Family name")]
        public string FamilyName { get; set; }

        [DisplayName("Date created from")]
        public DateTime CreatedDateFrom { get; set; }

        [DisplayName("To")]
        public DateTime CreatedDateTo { get; set; }
    }

    public class HomeModel
    {
    }


    #endregion

    #region Services
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.


    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
        void LoadPractitioner(string userName, int naatiNumber);
        void LoadRecertification(string userName, int naatiNumber);
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly ILogbookService mLogbookService;

        public FormsAuthenticationService(IPersonalDetailsService personalDetailsService, ILogbookService logbookService)
        {
            mPersonalDetailsService = personalDetailsService;
            mLogbookService = logbookService;
        }

        public void LoadPractitioner(string userName, int naatiNumber)
        {
            if (Roles.IsUserInRole(userName, SystemRoles.PRACTITIONER))
            {
                Roles.RemoveUserFromRole(userName, SystemRoles.PRACTITIONER);
            }

            if (Roles.IsUserInRole(userName, SystemRoles.FORMERPRACTITIONER))
            {
                Roles.RemoveUserFromRole(userName, SystemRoles.FORMERPRACTITIONER);
            }

            if (Roles.IsUserInRole(userName, SystemRoles.FUTUREPRACTITIONER))
            {
                Roles.RemoveUserFromRole(userName, SystemRoles.FUTUREPRACTITIONER);
            }

            var response =
                mPersonalDetailsService.GetPerson(new PersonNaatiNumberRequest
                {
                    NaatiNumber = naatiNumber
                });
            var person = response?.Person;

            if (person == null)
            {
                return;
            }

            if (person.IsPractitioner)
            {
                Roles.AddUserToRole(userName, SystemRoles.PRACTITIONER);
            }

            if (person.IsFormerPractitioner)
            {
                Roles.AddUserToRole(userName, SystemRoles.FORMERPRACTITIONER);
            }

            if (person.IsFuturePractitioner)
            {
                Roles.AddUserToRole(userName, SystemRoles.FUTUREPRACTITIONER);
            }
        }

        public void LoadRecertification(string userName, int naatiNumber)
        {
            if (Roles.IsUserInRole(userName, SystemRoles.RECERTIFICATION))
            {
                Roles.RemoveUserFromRole(userName, SystemRoles.RECERTIFICATION);
            }

            var credentials = mLogbookService.GetCredentials(naatiNumber).List;
            foreach (var credentialDto in credentials)
            {
                var response = mLogbookService.GetCredentialRecertificationStatus(credentialDto.Id);
                if (response.StatusId == (int)RecertificationStatus.EligibleForNew)
                {
                    Roles.AddUserToRole(userName, SystemRoles.RECERTIFICATION);
                    break;
                }
            }
        }

        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation

    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }

    #endregion
}
