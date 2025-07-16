using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.Payment;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Bl.Payment
{
    public class PaymentClient : IPaymentClient
    {
        private readonly ISecretsCacheQueryService _secretsProvider;
        private readonly ISecurePayAuthorisationService _securePayAuthorisationService;

        public PaymentClient(ISecretsCacheQueryService secretsProvider,
            ISecurePayAuthorisationService securePayAuthorisationService)
        {
            _secretsProvider = secretsProvider;
            _securePayAuthorisationService = securePayAuthorisationService; 
        }

//        private string GetCreditCardPaymentMessage(PaymentRequest request)
//        {
//            var message = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
//<SecurePayMessage>
//    <MessageInfo>
//        <messageID>{Guid.NewGuid():N}</messageID>
//        <messageTimestamp>{DateTime.Now:yyyyddMMHHmmssfff}000{GmtOffsetInMinutes}</messageTimestamp>
//        <timeoutValue>30</timeoutValue>
//        <apiVersion>xml-4.2</apiVersion>
//    </MessageInfo>
//    <MerchantInfo>
//        <merchantID>{MerchantId}</merchantID>
//        <password>{MerchantPassword}</password>
//    </MerchantInfo>
//    <RequestType>Payment</RequestType>
//    <Payment>
//        <TxnList count=""1"">
//            <Txn ID=""1"">
//                <txnType>0</txnType>
//                <txnSource>23</txnSource>
//                <amount>{(int)(Math.Round(request.Amount, 2) * 100)}</amount>
//                <purchaseOrderNo>{request.PurchaseOrderNo}</purchaseOrderNo>
//                <txnID>1</txnID>
//                <CreditCardInfo>
//                    <cardNumber>{request.CardNumber}</cardNumber>
//                    <cvv>{request.CardVerificationValue}</cvv>
//                    <expiryDate>{request.ExpiryMonth:MM/yy}</expiryDate>
//                </CreditCardInfo>
//            </Txn>
//        </TxnList>
//    </Payment>
//</SecurePayMessage>";

//            return message;
//        }

        private string GetRefundPaymentMessage(RefundPaymentRequest request)
        {
            var message = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SecurePayMessage>
    <MessageInfo>
        <messageID>{Guid.NewGuid():N}</messageID>
        <messageTimestamp>{DateTime.Now:yyyyddMMHHmmssfff}000{GmtOffsetInMinutes}</messageTimestamp>
        <timeoutValue>30</timeoutValue>
        <apiVersion>xml-4.2</apiVersion>
    </MessageInfo>
    <MerchantInfo>
        <merchantID>{MerchantId}</merchantID>
        <password>{MerchantPassword}</password>
    </MerchantInfo>
    <RequestType>Payment</RequestType>
    <Payment>
        <TxnList count=""1"">
            <Txn ID=""1"">
                <txnType>4</txnType>
                <txnSource>23</txnSource>
                <amount>{(int)(Math.Round(request.Amount, 2) * 100)}</amount>
                <purchaseOrderNo>{request.PurchaseOrderNo}</purchaseOrderNo>
                <txnID>{request.TransactionId}</txnID>              
            </Txn>
        </TxnList>
    </Payment>
</SecurePayMessage>";

            return message;
        }

        //public PaymentResponse MakePayment(PaymentRequest request)
        //{
        //    var message = GetCreditCardPaymentMessage(request);
        //    return SendSecurePayMessage(message, request.PurchaseOrderNo);
        //}

        public PaymentResponse MakePaymentEmbedded(PaymentRequestEmbedded request)
        {
            var paymentRequest = GetSecurePayPaymentRequest(request);
            return SendSecurePayRequest(request.PurchaseOrderNo, paymentRequest);
        }

        public PaymentResponse MakeRefundPaymentEmbedded(RefundPaymentRequest request)
        {
            var paymentRequest = GetSecurePayRefundRequest(request);
            return SendSecurePayRequest(request.PurchaseOrderNo, paymentRequest);
        }

        private HttpRequestMessage GetSecurePayPaymentRequest(PaymentRequestEmbedded request)
        {
            string token = GetSecurePayToken();
            var requestContent = JsonConvert.SerializeObject(new GatewayPaymentRequest
            {
                Amount = Convert.ToInt32(request.Amount * 100),
                MerchantCode = MerchantCode,
                Token = request.CreditCardToken,
                IP = OutboundIP,
                OrderId = request.PurchaseOrderNo
            });
            var paymentRequestMessage = new HttpRequestMessage(HttpMethod.Post, APIPaymentUrl)
            {
                Method = HttpMethod.Post,
                Headers = {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer " + token }
                },
                Content = new StringContent(requestContent),
                RequestUri = new Uri("/v2/payments", UriKind.Relative)
            };
            paymentRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return paymentRequestMessage;
        }

        private HttpRequestMessage GetSecurePayRefundRequest(RefundPaymentRequest request)
        {
            string token = GetSecurePayToken();

            var requestContent = JsonConvert.SerializeObject(new GatewayPaymentRequest
            {
                Amount = Convert.ToInt32(request.Amount * 100),
                MerchantCode = MerchantCode,
                IP = OutboundIP
            });
            var paymentRequestMessage = new HttpRequestMessage(HttpMethod.Post, APIPaymentUrl)
            {
                Method = HttpMethod.Post,
                Headers = {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer " + token }
                },
                Content = new StringContent(requestContent)
            };
            paymentRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            paymentRequestMessage.RequestUri = new Uri($"/v2/orders/{request.PurchaseOrderNo}/refunds", UriKind.Relative);

            return paymentRequestMessage;
        }

        private string GetSecurePayToken()
        {
            var token = _securePayAuthorisationService.GetFreshAccessToken();

            return token;
        }

        private PaymentResponse SendSecurePayRequest(string purchaseOrderNo, HttpRequestMessage requestMessage)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(APIPaymentUrl);
            var paymentResponse = new PaymentResponse();
            try
            {
                var paymentResponseContent = client.SendAsync(requestMessage).Result.Content.ReadAsStringAsync().Result;
                var paymentContent = JsonConvert.DeserializeObject<GatewayPaymentResponse> (paymentResponseContent);
                LoggingHelper.LogDebug("SecurePay response: {@SecurePayResponse}", paymentContent);

                //This allows for the situation where there are zero or many matching nodes
                var approved = paymentContent.Errors == null && paymentContent.Status.Equals("paid", StringComparison.OrdinalIgnoreCase);

                if (approved)
                {
                    paymentResponse.PaymentApproved = true;

                    var txnId = paymentContent.BankTransactionId;
                    paymentResponse.AssignedTransactionId = txnId;
                    paymentResponse.CardDetailsForReceipt = new CardDetails
                    {
                        CardNumber = string.Empty,
                        ExpiryMonth = string.Empty,
                        //Type = GetCardTypeFromId(responseDocument.SelectSingleNode("/SecurePayMessage/Payment/TxnList/Txn/CreditCardInfo/cardType")?.InnerText)
                    };
                }
                else
                {
                    LogRejectedPaymentEmbedded(purchaseOrderNo, paymentContent);

                    if (!string.IsNullOrEmpty(paymentContent.ErrorCode))
                    {
                        paymentResponse.PaymentErrorCode = paymentResponse.PaymentErrorDescription = paymentContent.ErrorCode;
                    }
                    else if (!string.IsNullOrEmpty(paymentContent.GatewayResponseCode))
                    {
                        paymentResponse.PaymentErrorCode = paymentContent.GatewayResponseCode;
                        paymentResponse.PaymentErrorDescription = paymentContent.GatewayResponseMessage; 
                    }
                    else if (paymentContent.Errors != null && paymentContent.Errors.Any())
                    {
                        paymentResponse.PaymentErrorCode = string.Join(",", paymentContent.Errors.Select(error => error.Code));
                        paymentResponse.PaymentErrorDescription = string.Join(",", paymentContent.Errors.Select(error => error.Detail));
                    }

                    if (paymentResponse.PaymentErrorCode == null)
                    {
                        if (paymentContent.Errors != null && paymentContent.Errors.Any())
                        {
                            var statusCode = string.Join(",", paymentContent.Errors.Select(error => error.Code));
                            var statusDescription = string.Join(",", paymentContent.Errors.Select(error => error.Detail));
                            paymentResponse.SystemErrorMessage = $"SecurePay call failed. {statusCode}: {statusDescription}";
                        }
                        else
                        {
                            paymentResponse.SystemErrorMessage = "SecurePay call failed. No useful information gathered from response.";
                        }                 
                    }
                }
            }
            catch (WebException wex)
            {
                paymentResponse.SystemErrorMessage = LogUnsuccessfulServiceCall(purchaseOrderNo, wex);
            }
            catch (Exception ex)
            {
                paymentResponse.SystemErrorMessage = LogGeneralError(purchaseOrderNo, ex);
            }
            return paymentResponse;
        }

        public PaymentResponse MakeRefundPayment(RefundPaymentRequest request)
        {
            var message = GetRefundPaymentMessage(request);
            return SendSecurePayMessage(message, request.PurchaseOrderNo);
        }

        private PaymentResponse SendSecurePayMessage(string message, string purchaseOrder)
        {
            var paymentResponse = new PaymentResponse();

            try
            {
                var responseDocument = GetResponseFromSecurePay(message);
                var approvalNodes = responseDocument.SelectNodes("/SecurePayMessage/Payment/TxnList/Txn/approved")?.Cast<XmlNode>().ToList() ?? new List<XmlNode>();
                //This allows for the situation where there are zero or many matching nodes
                var approved = approvalNodes.Count == 1 && approvalNodes.Single().InnerText.ToUpper() == "YES";

                if (approved)
                {
                    paymentResponse.PaymentApproved = true;

                    var txnId = responseDocument.SelectSingleNode("/SecurePayMessage/Payment/TxnList/Txn/txnID")?.InnerText;
                    paymentResponse.AssignedTransactionId = txnId;
                    paymentResponse.CardDetailsForReceipt = new CardDetails
                    {
                        CardNumber = responseDocument.SelectSingleNode("/SecurePayMessage/Payment/TxnList/Txn/CreditCardInfo/pan")?.InnerText,
                        ExpiryMonth = responseDocument.SelectSingleNode("/SecurePayMessage/Payment/TxnList/Txn/CreditCardInfo/expiryDate")?.InnerText,
                        Type = GetCardTypeFromId(responseDocument.SelectSingleNode("/SecurePayMessage/Payment/TxnList/Txn/CreditCardInfo/cardType")?.InnerText)
                    };
                }
                else
                {
                    LogRejectedPayment(purchaseOrder, responseDocument);
                    paymentResponse.PaymentErrorCode = responseDocument.SelectSingleNode("/SecurePayMessage/Payment/TxnList/Txn/responseCode")?.InnerText;
                    paymentResponse.PaymentErrorDescription = responseDocument.SelectSingleNode("/SecurePayMessage/Payment/TxnList/Txn/responseText")?.InnerText;

                    // if there is no Payment section in the response, check for a status code and assume an error condition
                    if (paymentResponse.PaymentErrorCode == null)
                    {
                        var statusCode = responseDocument.SelectSingleNode("/SecurePayMessage/Status/statusCode")?.InnerText;
                        var statusDescription = responseDocument.SelectSingleNode("/SecurePayMessage/Status/statusDescription")?.InnerText;
                        paymentResponse.SystemErrorMessage = "SecurePay call failed. "
                                                             + (statusCode == null
                                                                 ? "No useful information gathered from response."
                                                                 : $"{statusCode}: {statusDescription}");
                    }
                }
            }
            catch (WebException wex)
            {
                paymentResponse.SystemErrorMessage = LogUnsuccessfulServiceCall(purchaseOrder, wex);
            }
            catch (Exception ex)
            {
                paymentResponse.SystemErrorMessage = LogGeneralError(purchaseOrder, ex);
            }

            return paymentResponse;
        }

        private static CardType GetCardTypeFromId(string id)
        {
            int integerId;
            if (int.TryParse(id, out integerId) && Enum.IsDefined(typeof(CardType), integerId))
            {
                return (CardType)Enum.ToObject(typeof(CardType), integerId);
            }

            return CardType.Unknown;
        }

        private static string GmtOffsetInMinutes
        {
            get
            {
                var prefix = "+";
                var minutes = (int)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;

                if (minutes >= 0)
                {
                    return prefix + minutes;
                }

                prefix = "-";
                minutes *= -1;

                return prefix + minutes;
            }
        }

        private string MerchantId => _secretsProvider.Get("SecurePayMerchantId");
        private string MerchantCode => _secretsProvider.Get("SecurePayMerchantCode");

        private string MerchantPassword => _secretsProvider.Get("SecurePayMerchantPassword");

        private string OutboundIP => _secretsProvider.Get("OutboundIP");

        private static string PaymentUrl => ConfigurationManager.AppSettings["SecurePay_PaymentUrl"];

        private static string APIPaymentUrl => ConfigurationManager.AppSettings["SecurePayResource"];


        //private static string FailedPaymentEventLogTarget => ConfigurationManager.AppSettings["SecurePay_FailedPaymentEventLogTarget"];

        private static XmlDocument GetResponseFromSecurePay(string message)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(PaymentUrl);
            var encoding = new ASCIIEncoding();
            var bytes = encoding.GetBytes(message);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = bytes.Length;
            webRequest.Pipelined = false;
            webRequest.KeepAlive = false;

            // this is to set the security protocol to TLS 1.2, as securePay no longer accepts transport certificates below TLS 1.1 and TLS 1.1 does not seem to work
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            var requestStream = webRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            var webResponse = (HttpWebResponse)webRequest.GetResponse();

            var sr = new StreamReader(webResponse.GetResponseStream());
            var xmlreader = new XmlTextReader(sr)
            {
                WhitespaceHandling = WhitespaceHandling.None
            };

            var responseDocument = new XmlDocument();
            responseDocument.Load(xmlreader);

            sr.Close();
            xmlreader.Close();

            webResponse.Close();

            return responseDocument;
        }

        public string LogRejectedPayment(string purchaseOrder, XmlDocument response)
        {
            try
            {
                var stringWriter = new StringWriter();
                var writer = new XmlTextWriter(stringWriter)
                {
                    Formatting = System.Xml.Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                };

                var redactedResponse = CopyAndRedactRemotePaymentResponse(response);
                redactedResponse.WriteTo(writer);

                var entryText = string.Format("Payment was not approved for purchase order '{0}'.{1}Redacted response received:{1}{2}",
                    purchaseOrder,
                    Environment.NewLine,
                    stringWriter);

                LoggingHelper.LogWarning("Secure pay  error {SecurePayError}", entryText);
                return "Logged response to application event log ";
            }
            catch
            {
                return "Could not log response";
            }
        }

        public string LogRejectedPaymentEmbedded(string purchaseOrder, GatewayPaymentResponse response)
        {
            try
            {
                string errorSummary = JsonConvert.SerializeObject(response.Errors);

                var entryText = string.Format("Payment was not approved for purchase order '{0}'.{1}Redacted response received:{1}{2}",
                    purchaseOrder,
                    Environment.NewLine,
                    errorSummary);

                LoggingHelper.LogWarning("Secure pay  error {SecurePayError}", entryText);
                return "Logged response to application event log ";
            }
            catch
            {
                return "Could not log response";
            }
        }

        //Let's avoid dumping unencrypted credit card details into the event log.
        private static XmlDocument CopyAndRedactRemotePaymentResponse(XmlDocument input)
        {
            var copy = (XmlDocument)input.Clone();
            ModifyNodeText(copy, "/SecurePayMessage/Payment/TxnList/Txn/CreditCardInfo/pan", s =>
                {
                    if (s.Length <= 6)
                        return s;

                    return new string('X', s.Length - 6) + s.Substring(s.Length - 6, 6);
                });
            ModifyNodeText(copy, "/SecurePayMessage/Payment/TxnList/Txn/CreditCardInfo/expiryDate", s => new string('X', s.Length));
            return copy;
        }

        private static void ModifyNodeText(XmlDocument input, string xpath, Func<string, string> doModification)
        {
            //Remember: For some errors the xpath may select zero nodes
            var matchingNodes = input.SelectNodes(xpath)?.Cast<XmlNode>() ?? Enumerable.Empty<XmlNode>();
            foreach (var node in matchingNodes)
            {
                node.InnerText = doModification(node.InnerText);
            }
        }

        private static string LogUnsuccessfulServiceCall(string purchaseOrder, WebException wex)
        {
            try
            {
                var entryText = string.Format("Could not call SecurePay service for purchase order '{0}'.{1}Url: {3}{1}Exception:{1}{2}",
                    purchaseOrder,
                    Environment.NewLine,
                    wex,
                    PaymentUrl);

                LoggingHelper.LogWarning("Secure pay  error {SecurePayError}", entryText);
                return $"Logged exception to application event log";
            }
            catch (Exception)
            {
                return $"Could not log exception. Exception was {wex}";
            }
        }

        private static string LogGeneralError(string purchaseOrder, Exception ex)
        {
            try
            {
                var entryText = string.Format("Unexpected error encountered when paying for purchase order '{0}'.{1}Exception:{1}{2}",
                    purchaseOrder,
                    Environment.NewLine,
                    ex);

                LoggingHelper.LogWarning("Secure pay  error {SecurePayError}", entryText);
                return $"Logged exception to application event log";
            }
            catch (Exception)
            {
                return $"Could not log exception. Exception was {ex}";
            }
        }
    }
}
