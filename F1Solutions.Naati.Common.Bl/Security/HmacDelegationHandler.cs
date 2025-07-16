using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Bl.Security
{
    public class HmacDelegatingHandler : DelegatingHandler
    {
        private readonly string _privateKey;
        private readonly string _publicKey;
        private readonly string _scheme;

        public HmacDelegatingHandler(string privateKey, string publicKey, string scheme)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
            _scheme = scheme;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var base64HashedRequestContent = string.Empty;
            var requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());
            var requestHttpMethod = request.Method.Method;

            // calculate unix time
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.UtcNow - epochStart;
            var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            // create random nonce for each request
            var nonce = Guid.NewGuid().ToString("N");

            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                var md5 = MD5.Create();

                // any change in request body will result in different hash, violating message integrity
                var hashedRequestContent = md5.ComputeHash(content);
                base64HashedRequestContent = Convert.ToBase64String(hashedRequestContent);
            }

            byte[] secretKeyBytes = Convert.FromBase64String(_privateKey);
            string signatureRawData = $"{_publicKey}{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}{base64HashedRequestContent}";
            byte[] signatureBytes = Encoding.UTF8.GetBytes(signatureRawData);
            using (var hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] hashedSignature = hmac.ComputeHash(signatureBytes);
                string requestSignatureBase64String = Convert.ToBase64String(hashedSignature);

                // create the auth header
                request.Headers.Authorization = new AuthenticationHeaderValue(_scheme, $"{_publicKey}:{requestSignatureBase64String}:{nonce}:{requestTimeStamp}");
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
