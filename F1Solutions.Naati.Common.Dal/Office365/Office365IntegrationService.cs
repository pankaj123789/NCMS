using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace F1Solutions.Naati.Common.Dal.Office365
{
    public class Office365IntegrationService
    {
        private GraphServiceClient _apiInstance;
        private readonly string _apiBaseUrl;
        private readonly IAzureAuthorisationService _authService;
        private string _accessToken;

        public Office365IntegrationService(IAzureAuthorisationService authService)
        {
            _authService = authService;
            _apiBaseUrl = ConfigurationManager.AppSettings["GraphApiBaseUrl"];
        }

        public string AccessToken
        {
            get
            {
                if (String.IsNullOrEmpty(_accessToken))
                {
                    _accessToken = _authService.GetFreshAccessToken();
                }
                return _accessToken;
            }
        }

        public GraphServiceClient GraphApiInstance
        {
            get
            {
                if (_apiInstance == null)
                {
                    _apiInstance = new GraphServiceClient(_apiBaseUrl,
                        new DelegateAuthenticationProvider(
                            requestMessage =>
                            {
                                // append the access token to the request as a bearer token
                                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                                return Task.FromResult(0);
                            }));
                }

                return _apiInstance;
            }
        }

        private (bool Error, string Message, JObject Result) ProcessQuery(HttpClient client, string query)
        {
            HttpResponseMessage response = client.GetAsync($"{_apiBaseUrl}/{query}").Result;

            response.NotNull(nameof(response));

            var responseString = response.Content.ReadAsStringAsync().Result;
            responseString.NotNullOrWhiteSpace("response from GRAPH");

            var jObj = JObject.Parse(responseString);

            string message = string.Empty;

            var error = jObj["error"];
            if (error != null)
            {
                var apiMessage = error["message"]?.ToString();
                var code = error["code"]?.ToString();
                message =
                    !String.IsNullOrEmpty(apiMessage)
                        ? apiMessage
                        : !String.IsNullOrEmpty(code)
                            ? code
                            : "no reason given";

                LoggingHelper.LogWarning("Graph API error. Message: {Message}; Code: {Code}; ReasonPhrase: {ReasonPhrase}; Query: {Query}", apiMessage, code, response.ReasonPhrase, query);
            }

            return (error != null, message, jObj);
        }

        public IDictionary<string, IEnumerable<Message>> GetMailMessagesFromSharedAccount(List<string> sharedAccounts, string emailAddress)
        {
            var emailsDictionary = new Dictionary<string, IEnumerable<Message>>(StringComparer.OrdinalIgnoreCase);
            if (!sharedAccounts.Any())
                return null;

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
           
            foreach (var account in sharedAccounts)
            {
                var listMessage = new List<Message>();
                if (String.IsNullOrWhiteSpace(account))
                {
                    continue;
                }

                var result = ProcessQuery(client,
                    $"users/{account}/mailFolders/AllItems/messages?$search=\"participants:{emailAddress}\"&$select=from,subject,HasAttachments,Id,SentDateTime&$top=200");

                if (result.Error)
                {
                    continue;
                }

                var resultData = result.Result["value"]?.ToString();

                if (resultData == null || resultData == "[]")
                {
                    continue;
                }

                listMessage.AddRange(JsonConvert.DeserializeObject<Message[]>(resultData));

                emailsDictionary[account] = listMessage;
            }
            return emailsDictionary;
        }

        public Task<HttpResponseMessage> SendEmail(Message message, bool saveEmail)
        {
            var client = new HttpClient();

            var email = new { message = message, saveToSentItems = saveEmail };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var jsonString = JsonConvert.SerializeObject(email);
            var content = new StringContent(jsonString, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var fromEmailAddress = email.message.From.EmailAddress.Address?.Trim();
           
            var task = client.PostAsync($"{_apiBaseUrl}/users/{fromEmailAddress}/sendMail", content);
            return task;
        }

        public IEnumerable<FileAttachment> GetEmailAttachment(string mailId, string emailAddress)
        {
            var client = new HttpClient();
            var listMessage = new List<FileAttachment>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var response =
                client.GetAsync(
                        $"{_apiBaseUrl}/users/{emailAddress}/messages/{mailId}/attachments")
                    .Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            if (JObject.Parse(responseString)["value"].ToString() == "[]")
                return listMessage;

            foreach (var item in JObject.Parse(responseString)["value"])
            {
                // this is for future or custom screen to read item attachment.
                //if (item["@odata.type"].ToString() == "#microsoft.graph.itemAttachment")
                //{
                //var id = item["id"];
                //var itemResponse =
                //client.GetAsync(
                //        $"https://outlook.office365.com/api/v1.0/me/messages/{mailId}/attachments/{id}?$expand=Microsoft.OutlookServices.ItemAttachment/Item")
                //    .Result;
                //var itemResponseString = itemResponse.Content.ReadAsStringAsync().Result;

                //listMessage.Add(JsonConvert.DeserializeObject<ItemAttachment>(item.ToString()));
                //}
                if (item["@odata.type"].ToString() == "#microsoft.graph.fileAttachment")
                {
                    listMessage.Add(JsonConvert.DeserializeObject<FileAttachment>(item.ToString()));
                }
            }
            return listMessage;
        }

        public Message GetEmailDetails(string mailId, string emailAddress)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var response =
                client.GetAsync(
                        $"{_apiBaseUrl}/users/{emailAddress}/messages/{mailId}")
                    .Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            responseString.NotNullOrWhiteSpace("response from GRAPH");
            var jObj = JObject.Parse(responseString);

            var error = jObj["error"];
            if (error != null)
            {
                var message = error["message"]?.ToString();
                var code = error["code"]?.ToString();
                throw new Exception($"Office 365 error accessing email id for user {mailId}: {message ?? code ?? "no reason given"}");
            }
            var email = JsonConvert.DeserializeObject<Message>(jObj.ToString());
            if (!(email.ToRecipients?.Any() ?? false))
            {
                LoggingHelper.LogWarning("No recipients found for emailGraphId {EmailGraphId}. Message {EmailMessage}", mailId, jObj.ToString());
            }
            return email;
        }
    }
}


