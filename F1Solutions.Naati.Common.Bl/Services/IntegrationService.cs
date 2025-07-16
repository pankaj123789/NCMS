using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Bl.Services
{
    public abstract class IntegrationService
    {
        private static bool? _debugMode;

        protected static bool DebugMode => (_debugMode ?? (_debugMode = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugMode"]))).Value;
        protected abstract string BaseAddress { get; }

        protected abstract string AuthenticationScheme { get; }

        protected R SendPostRequest<T, R>(T requestData, string url, [CallerMemberName] string operationName = null)
            where R : BusinessServiceResponse
        {
            return SendPostRequestAsync<T,R>(requestData, url, operationName).Result;
        }

        protected async Task<R> SendPostRequestAsync<T, R>(T requestData, string url, [CallerMemberName] string operationName = null)
            where R : BusinessServiceResponse
        {
            var data = JsonConvert.SerializeObject(requestData);
            var ctn = new StringContent(data, Encoding.UTF8, "application/json");

            var privateKey = GetPrivateKey();
            var publicKey = GetPublicKey();

            var customDelegatingHandler = new HmacDelegatingHandler(privateKey, publicKey, AuthenticationScheme);

            using (var client = HttpClientFactory.Create(customDelegatingHandler))
            {
                var requestUrl = BaseAddress;
                if (!requestUrl.EndsWith("/"))
                {
                    requestUrl = requestUrl + "/";
                }

                requestUrl = string.Concat(requestUrl, url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Stopwatch stopWatch = null;
                if (DebugMode)
                {
                    stopWatch = Stopwatch.StartNew();
                }

                var response = await client.PostAsync(requestUrl, ctn).ConfigureAwait(false);

                if (stopWatch != null)
                {
                    stopWatch.Stop();
                    LoggingHelper.LogWarning("NCMS Private Call duration in ms:{duration}, endpoint: {endpoint} ", stopWatch.Elapsed.TotalMilliseconds, operationName);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return await GetErrorResponseAsync<R>(response, operationName).ConfigureAwait(false);
                }
                return await GetMessageResponseAsync<R>(response).ConfigureAwait(false);
            }
        }

        private async Task<TR> GetMessageResponseAsync<TR>(HttpResponseMessage requestResponse) where TR : BusinessServiceResponse
        {
            var msg = await requestResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<TR>(msg);
            return result;
        }

        private async Task<TR> GetErrorResponseAsync<TR>(HttpResponseMessage requestResponse, string method)
            where TR : BusinessServiceResponse
        {
            var response = Activator.CreateInstance<TR>();
            var errors = await GetErrorsAsync(requestResponse);
            var builder = new StringBuilder();
            builder.AppendLine("Error executing {className} request.");
            builder.AppendLine("Error occurred while executing: {methodName} ");
            builder.AppendLine("URL: {url}");
            builder.AppendLine("Response:");
            builder.AppendLine(response.ToString());
            LoggingHelper.LogError(
                builder.ToString(),
                GetType().Name,
                method,
                requestResponse.RequestMessage?.RequestUri,
                errors);

            var responseData = Activator.CreateInstance<TR>();
            responseData.Success = false;
            responseData.Errors = errors;
            return responseData;
        }

        private async Task<List<string>> GetErrorsAsync(HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var errors = new List<string>();
            try
            {
                errors.AddRange(JsonConvert.DeserializeObject<IList<string>>(data));
            }
            catch
            {
                errors.Add(data);
            }

            return errors;
        }

        protected abstract string GetPrivateKey();
        protected abstract string GetPublicKey();
    }
}