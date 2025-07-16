using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Security;
using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Bl.Security
{
    public class RecaptchaValidatorHelper : IRecaptchaValidatorHelper
    {
        private readonly ISecretsCacheQueryService _secretsProvider;

        public RecaptchaValidatorHelper(ISecretsCacheQueryService secretsProvider)
        {
            _secretsProvider = secretsProvider;
        }

        private bool SkipRecaptcha()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["kosettings:skipRecaptcha"]);
        }

        public bool IsValidRecaptcha(string captchaResponse)
        {
            if (SkipRecaptcha())
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(captchaResponse))
            {
                return false;
            }

            var recaptchaSecretKey = _secretsProvider.Get("RecaptchaSecretKey");

            var recaptchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(
                new WebClient().DownloadString($"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecretKey}&response={captchaResponse}"));

            return recaptchaResponse.Success;
        }
    }

    public class RecaptchaResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
    }