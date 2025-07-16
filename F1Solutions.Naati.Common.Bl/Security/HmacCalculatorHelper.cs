using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Naati.Common.Bl.Security
{
    public class HmacCalculatorHelper
    {

        public static (string appId, string base64Signature, string nonce, string requestTimeStamp)
            GetAuthorizationHeaderValues(string rawAuthzHeader)
        {
            var credArray = rawAuthzHeader?.Split(':') ?? new string[] { };

            if (credArray.Length != 4)
            {
                return (null, null, null, null);
            }

            return (credArray[0], credArray[1], credArray[2], credArray[3]);
        }

        public static async Task<bool> IsValidRequest(
            HttpRequestMessage req,
            string appId,
            string incomingBase64Signature,
            string nonce,
            string requestTimeStamp,
            string privateKey, bool isExternalRequest)
        {
            var hash = await ComputeHash(req.Content);
            var requestContentBase64String = "";
            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            var secretKeyBytes = Convert.FromBase64String(privateKey);

            //the decode has been added due to an inconsistency with the util. it should not be required.

            var requestUri = HttpUtility.UrlEncode(req.RequestUri.AbsoluteUri.ToLower());

            //  Https is converted to http by the firewall.
            if (isExternalRequest && Convert.ToBoolean(ConfigurationManager.AppSettings["HttpsConfigured"]) && !requestUri.StartsWith("https"))
            {
                var searchedString = "http";
                var index = requestUri.IndexOf(searchedString, StringComparison.Ordinal);
                if (index == 0)
                {
                    requestUri = "https" + requestUri.Substring(searchedString.Length);
                }
            }

            var requestHttpMethod = req.Method.Method;

            var signature = Encoding.UTF8.GetBytes(
                $"{appId}{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}{requestContentBase64String}");

            //LoggingHelper.LogInfo($"PrivateKey: {privateKey}");

            using (var hmac = new HMACSHA256(secretKeyBytes))
            {
                var signatureBytes = hmac.ComputeHash(signature);

                var result = incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal);

                if (!result)
                {
                    LoggingHelper.LogInfo($"appId: {appId}");
                    LoggingHelper.LogInfo($"requestHttpMethod: {requestHttpMethod}");
                    LoggingHelper.LogInfo($"URL: {requestUri}");
                    LoggingHelper.LogInfo($"nonce: {nonce}");
                    LoggingHelper.LogInfo($"requestTimeStamp: {requestTimeStamp}");
                    LoggingHelper.LogInfo($"requestContentBase64String: {requestContentBase64String}");
                }

                return result;
            }
        }

        private static async Task<byte[]> ComputeHash(HttpContent httpContent)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = await httpContent.ReadAsByteArrayAsync();
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }

                return hash;
            }
        }

        public static string GenerateProtectedKey(params string[] purpose)
        {
            var newKey = Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray();
            var encryptedKey = MachineKey.Protect(newKey, purpose);
            return Convert.ToBase64String(encryptedKey);
        }

        public static string UnProtectKey(string protectedKey, params string[] purpose)
        {
            var plainTextBytes = Convert.FromBase64String(protectedKey);
            var decrypted = MachineKey.Unprotect(plainTextBytes, purpose);
            var descryptedKey = Convert.ToBase64String(decrypted ?? new byte[0]);
            return descryptedKey;
            //if (decrypted != null)
            //{
            //    return Encoding.UTF8.GetString(decrypted).Replace("\0", "");
            //}

            //return string.Empty;
        }
    }
}