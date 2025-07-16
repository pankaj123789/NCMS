using System.Web.Mvc;
using MyNaati.Ui.Controllers;
using MyNaati.Ui.Models;

namespace MyNaati.Ui.Helpers
{
    public interface IRegisterHelper
    {
        void ValidateRegistrationInputs(IRegisterModel model, ModelStateDictionary modelState);

        CreateUserResponse CreateUser(int naatiNumber, string primaryEmail);

        void RegisterEmailChange(string currentPrimaryEmail, string newEmail, string emailUrl, int emailChangeValidHours);

        bool EmailsMatch(string first, string second);

        string ValidateRegistrationRequest(string email, int naatiNumber);

        void ValidateCaptcha(bool captchaValid, string captchaErrorMessage, ModelStateDictionary modelState);

        ChangeLogonEmailResponse ChangeLogOnEmail(int reference, string password, int emailChangeValidHours);

        bool HasPendingPrimaryEmailChange(string primaryEmail, int emailChangeValidHours);

        GetRegisteredEmailChangeResponse GetRegisteredEmailChange(int reference, int emailChangeValidHours);
    }
}
