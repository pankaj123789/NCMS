using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using MyNaati.Contracts.BackOffice;
using System;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.BackOffice
{
    public class EmailCodeVerificationService : IEmailCodeVerificationService
    {
        private IPersonEmailVerificationCodeDalService mPersonEmailVerificationDalService;
        private IPersonQueryService mPersonQueryService;
        private ILookupProvider mLookupProvider;
        private readonly IEmailService mEmailService;

        public EmailCodeVerificationService(IPersonEmailVerificationCodeDalService personEmailVerificationDalService,
            IPersonQueryService personQueryService, ILookupProvider lookupProvider, IEmailService emailService)
        {
            mPersonEmailVerificationDalService = personEmailVerificationDalService;
            mPersonQueryService = personQueryService;
            mLookupProvider = lookupProvider;
            mEmailService = emailService;
        }

        public BusinessServiceResponse SendEmailAccessCode(int naatiNumber)
        {
            var getAccessCodeResponse = GetNewEmailVerificationCode(naatiNumber);
            if(!getAccessCodeResponse.Success)
            {
                LoggingHelper.LogError("Error getting Access Code; NAATI No. {NaatiNumber}; {Error}", naatiNumber, getAccessCodeResponse.Errors.FirstOrDefault());
                return getAccessCodeResponse;
            }

            var accessCode = getAccessCodeResponse.Data.AccessCode;
            var emailAddress = getAccessCodeResponse.Data.EmailAddress;

            var emailRequest = new SendEmailRequest(EmailTemplate.SendAccessCode, emailAddress);
            emailRequest.Tokens.Add(EmailTokens.AccessCode, accessCode.ToString());
            emailRequest.Tokens.Add(EmailTokens.NaatiNumber, naatiNumber.ToString());
            emailRequest.Tokens.Add(EmailTokens.AccessCodeValidity, mLookupProvider.SystemValues.EmailAccessCodeValidityMinutes.ToString());
            var emailResponse = mEmailService.SendMail(emailRequest);

            if (!emailResponse.Success)
            {
                LoggingHelper.LogError("Error emailing Access Code; NAATI No. {NaatiNumber}; {Error}", naatiNumber, emailResponse.Exception);
                return BusinessServiceResponse.Failed;
            }

            return BusinessServiceResponse.Succeeded;
        }

        public GenericResponse<NewEmailVerificationCodeResponse> GetNewEmailVerificationCode(int naatiNumber)
        {
            var newEmailCode = GenerateNewEmailCode();
            var setLastEmailResponse = mPersonEmailVerificationDalService.SetLastEmailCode(naatiNumber, newEmailCode);

            if(!setLastEmailResponse.Success)
            {
                return new GenericResponse<NewEmailVerificationCodeResponse>().Absorb(setLastEmailResponse);
            }

            var emailAddress = setLastEmailResponse.Data;

            return new NewEmailVerificationCodeResponse{
                AccessCode = newEmailCode,
                EmailAddress = emailAddress,
                NaatiNumber = naatiNumber.ToString()
            };
        }

        public GenericResponse<bool> VerifyEmailVerificationCode(int naatiNumber, int emailCode)
        {
            var mfaResponse = mPersonQueryService.GetPersonMfaDetails(naatiNumber);

            if (!mfaResponse.Success)
            {
                return new GenericResponse<bool>().Absorb(mfaResponse);
            }

            var mfaDetails = mfaResponse.Data;
            var response = new GenericResponse<bool>();

            if (mfaDetails.EmailCode == 0 || !mfaDetails.EmailCodeExpireStartDate.HasValue)
            {
                response.Errors.Add("No access code has been sent.");
                response.Success = false;
                return response;
            }

            if (mfaDetails.EmailCodeExpireStartDate.Value.AddMinutes(mLookupProvider.SystemValues.EmailAccessCodeValidityMinutes) < DateTime.Now)
            {
                response.Messages.Add("The access code has expired. Please request a new code.");
                return response;
            }

            if (emailCode != mfaDetails.EmailCode)
            {
                response.Messages.Add("Please check the code and try again.");
                return response;
            }

            var request = new PersonMfaRequest
            {
                NaatiNumber = naatiNumber,
                MfaExpireStartDate = DateTime.Now,
            };
            var saveResult = mPersonQueryService.SetPersonMfaDetails(request);

            if (!saveResult.Success)
            {
                return new GenericResponse<bool>().Absorb(saveResult);
            }

            response.Data = true;
            return response;
        }

        private int GenerateNewEmailCode()
        {
            var rnd = new Random();
            var newNumber = rnd.Next(111111, 999999);
            return newNumber;
        }
    }
}