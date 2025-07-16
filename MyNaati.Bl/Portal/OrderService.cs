using System;
using System.Transactions;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.Payment;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common.PDListing;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using MyNaati.Contracts.Portal;
using CardType = MyNaati.Contracts.Portal.CardType;
using PaymentRequest = MyNaati.Contracts.Portal.PaymentRequest;
using PaymentResponse = MyNaati.Contracts.Portal.PaymentResponse;

namespace MyNaati.Bl.Portal
{
    public class OrderService : IOrderService
    {
        private readonly IPaymentClient mPaymentClient;
        
        public OrderService(
            IPaymentClient paymentClient)
        {
            mPaymentClient = paymentClient;
        }

        public PaymentResponse SubmitCreatePayment(PaymentRequest request)
        {

            var result = new PaymentResponse();
            F1Solutions.Naati.Common.Contracts.Bl.Payment.PaymentResponse paymentResponse = null;

            string purchaseOrder = null; 
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {

                    //Payment is done last because it can't be rolled back (easily)
                    if (!string.IsNullOrEmpty(request.InvoiceNumber))
                    {
                        purchaseOrder = $"{request.NAATINumber}-{request.InvoiceNumber}-{DateTime.Now:ddMMyy-HHmmssff}";
                        paymentResponse = MakeCreditCardPaymentEmbedded(purchaseOrder, request.Amount, request.CardDetails);
                    }
                    else
                    {
                        purchaseOrder = $"{request.NAATINumber}-APP{request.ApplicationId}-{DateTime.Now:ddMMyy-HHmmssff}";
                        paymentResponse = MakeCreditCardPaymentEmbedded(purchaseOrder, request.Amount, request.CardDetails);
                    }

                    if (paymentResponse.PaymentApproved)
                    {
                        result.PaymentStatus = PaymentProcessStatus.Succeeded;

                        result.CardDetailsForReceipt = new ReturnedCardDetails
                        {
                            CardNumber = paymentResponse.CardDetailsForReceipt.CardNumber,
                            ExpiryMonth = paymentResponse.CardDetailsForReceipt.ExpiryMonth,
                            Type = Translate(paymentResponse.CardDetailsForReceipt.Type)
                        };

                        result.Success = true;
                        result.UnHandledException = false;
                        scope.Complete();
                    }
                    else
                    {
                        result.Success = false;
                        result.PaymentStatus = PaymentProcessStatus.Failed;
                        result.PaymentFailureDetails = new PaymentFailureDetails
                        {
                            SystemError = !string.IsNullOrEmpty(paymentResponse.SystemErrorMessage),
                            SystemErrorMessage = paymentResponse.SystemErrorMessage,
                            RejectedPayment = string.IsNullOrEmpty(paymentResponse.SystemErrorMessage),
                            RejectionCode = paymentResponse.PaymentErrorCode,
                            RejectionDescription = paymentResponse.PaymentErrorDescription,
                        };
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;

                    result.UnHandledException = true;

                    if (result.PaymentStatus == PaymentProcessStatus.Succeeded)
                    {
                        scope.Complete();
                        result.InvoiceNumber = !string.IsNullOrEmpty(request.InvoiceNumber) ? request.InvoiceNumber : string.Empty;
                        result.ApplicationId = !string.IsNullOrEmpty(request.ApplicationId) ? request.ApplicationId : string.Empty;
                    }
                    else
                    {
                        var concatExceptionMessage = !string.IsNullOrEmpty(request.InvoiceNumber) ? 
                            "Invoice Number: " + request.InvoiceNumber + ",  Customer Number: " + request.NAATINumber : 
                            "Applciation Id: " + request.ApplicationId + ",  Customer Number: " + request.NAATINumber;

                        result.UnHandledExceptionMessage =
                            "We could not process your payment. Please try again and if the problem persists, please contact NAATI's finance team at finance@naati.com.au. Please quote the following information:" +
                            concatExceptionMessage;
                    }
                    try
                    {
                        LoggingHelper.LogException(ex, "Error applying payment.");
                    }
                    catch 
                    {
                        //Deliberately ignore
                    }
                    
                }
            }

            if (paymentResponse != null && paymentResponse.PaymentApproved)
            {
                result.OrderNumber = purchaseOrder; 
                result.ReferenceNumber = paymentResponse.AssignedTransactionId;
            }
            
            return result;
        }

        public void CompleteFreePDListingOrder(PractitionerDirectoryUpdateListingRequest pdUpdate)
        {
            PublishPdListingUpdate(pdUpdate);
        }

        private F1Solutions.Naati.Common.Contracts.Bl.Payment.PaymentResponse MakeCreditCardPaymentEmbedded(string referenceNumber, decimal amount, EnteredCardDetails cardDetails)
        {
            var paymentRequest = new PaymentRequestEmbedded
            {
                PurchaseOrderNo = referenceNumber,
                Amount = amount,

                CreditCardToken = cardDetails.CardToken
            };

            return mPaymentClient.MakePaymentEmbedded(paymentRequest);
        }

        private void PublishOrderConfirmation(Order order, EnteredCardDetails cardDetails, CardDetails paymentResponseCardDetails)
        {
            throw new NotSupportedException();
        }

        private void PublishPdListingUpdate(PractitionerDirectoryUpdateListingRequest pdUpdate)
        {
            throw new  NotSupportedException();
        }

        private static CardType Translate(F1Solutions.Naati.Common.Contracts.Bl.Payment.CardType input)
        {
            CardType result;
            return Enum.TryParse(input.ToString(), true, out result) ? result : CardType.Unknown;
        }
    }
}
