using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;

namespace MyNaati.Contracts.BackOffice
{
    public interface IEmailCodeVerificationService:IInterceptableservice
    {
        BusinessServiceResponse SendEmailAccessCode(int naatiNumber);

        /// <summary>
        /// Generates a new code
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns>email address to send </returns>
        GenericResponse<NewEmailVerificationCodeResponse> GetNewEmailVerificationCode(int naatiNumber);

        /// <summary>
        /// Verify that the supplied Email Verification Code mataches the DB
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns></returns>
        GenericResponse<bool> VerifyEmailVerificationCode(int naatiNumber, int emailCode);
    }

    public class NewEmailVerificationCodeResponse
    {
        public string EmailAddress { get; set; }
        public int AccessCode { get; set; }
        public string GivenName { get; set; }
        public string NaatiNumber { get; set; }


    }
}