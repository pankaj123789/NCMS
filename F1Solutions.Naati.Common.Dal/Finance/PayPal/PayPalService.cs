using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Finance.PayPal
{
    public class PayPalService : IPayPalService
    {
        private Payment payment;

        public const string PAYPALORDERPREFIX = "PAYID";
        public const string PAYPALTRANSPREFIX = "PAYPAL";

        public Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            LoggingHelper.LogInfo($"PayPal before Execute Payment. PayerId: {payerId}, Payment Reference: {paymentId}");
            var payment =  this.payment.Execute(apiContext, paymentExecution);
            LoggingHelper.LogInfo($"PayPal after Execute Payment. PayerId: {payerId}, Payment Reference: {paymentId}, Amount: {payment.transactions?[0].amount?.total}");
            return payment;
        }


        public DetailedRefund ExecuteRefund(APIContext apiContext, string amount, string saleId)
        {
            var decAmount = decimal.Parse(amount);
            var formattedAmount = String.Format("{0:0.00}", decAmount);
            var refund = new RefundRequest()
            {
                amount = new Amount() { currency = "AUD", total = formattedAmount }
            };
            LoggingHelper.LogInfo($"PayPal Create Refund. Payment Reference: {saleId}, Amount {amount}");
            var detailedRefund = Sale.Refund(apiContext, saleId.Replace($"{PAYPALTRANSPREFIX}-", ""), refund);
            return detailedRefund;
        }

        public Payment CreatePayment(APIContext apiContext, string redirectUrl, string paymentReference, string unitType, string balance)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
                {
                    new Item()
                    {
                        name = paymentReference,
                        currency = "AUD",
                        price = balance,
                        quantity = "1",
                        sku = unitType
                    }
                }
            };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            var transactionList = new List<Transaction>()
            {
                new Transaction()
                {
                    description = "Fee",
                    invoice_number = paymentReference,
                    amount = new Amount()
                    {
                        currency = "AUD",
                        total = balance,
                    },
                    item_list = itemList
                }
            };

            payment = new Payment()
            {
                intent = "sale",
                payer = new Payer()
                {
                    payment_method = "paypal"
                },
                transactions = transactionList,
                redirect_urls = redirUrls,
            };

            // Create a payment using a APIContext  
            LoggingHelper.LogInfo($"PayPal Create Payment. Payment Reference: {paymentReference}, Amount {balance}");
            return payment.Create(apiContext);
        }

    }

    public class PayPalError
    {
        public string name { get; set; }
        public string message { get; set; }
    }
}

