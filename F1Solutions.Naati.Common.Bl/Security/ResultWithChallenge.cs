using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace F1Solutions.Naati.Common.Bl.Security
{
    public class ResultWithChallenge : IHttpActionResult
    {

        private readonly IHttpActionResult _mNext;
        private readonly string _authenticationScheme;

        public ResultWithChallenge(IHttpActionResult next, string authenticationScheme)
        {
            _mNext = next;
            _authenticationScheme = authenticationScheme;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await _mNext.ExecuteAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(_authenticationScheme));
            }

            return response;
        }
    }
}