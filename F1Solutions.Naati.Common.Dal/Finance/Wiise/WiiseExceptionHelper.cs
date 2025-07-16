using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using System;
using System.ComponentModel.DataAnnotations;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{

    internal static class WiiseExceptionHelper
    {
        private static string GetAdviceString(string message)
        {
            var parts = message.Split(new[] { "advice=" }, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                return parts[1];
            }
            return message;
        }

        public static string GetMessage(ApiException wiiseException)
        {
            string message;
            var vEx = wiiseException as ValidationException;
            if (vEx != null)
            {
                message = "Wiise validation exception(s): " + vEx.ValidationResult.ErrorMessage;
            }
            else
            {
                if (wiiseException.IsRateExceededException())
                {
                    message = "Wiise rate limit exceeded.";
                }
                else
                {
                    message = $"An error occurred while communicating with Wiise. {GetAdviceString(wiiseException.Message)}";
                }
            }
            return message.Replace("%20", " ");

        }

        public static string GetMessage(Exception exception)
        {
            var xEx = exception as ApiException;
            return xEx != null ? GetMessage(xEx) : exception.Message;
        }



        public class WiiseApiKeyException : WebServiceException
        {
            public WiiseApiKeyException(string message)
                : base(message)
            {
            }
        }

        public class WiiseException : WebServiceException
        {
            public WiiseException(string message)
                : base(message)
            {
            }

            public WiiseException(Exception ex)
                : base("An error occurred while communicating with Wiise. See inner exception.", ex)
            {
            }
        }
    }
}

