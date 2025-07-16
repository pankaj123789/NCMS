namespace F1Solutions.Naati.Common.Contracts.Security
{
    public interface IRecaptchaValidatorHelper
    {
        bool IsValidRecaptcha(string captchaResponse);
    }
}
