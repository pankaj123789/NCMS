using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    internal static class PaymentMapper
    {

        internal static List<NativeModels.JournalLine> ToNativeLineItems(this List<PublicModels.Payment> payments)
        {
            var nativeLineItems = new List<NativeModels.JournalLine>();

            var linenumber = 0;
            foreach (var payment in payments)
            {
                if (payment.HasValidationErrors)
                {
                    var baseModel = payment.ToNativeBaseModel();
                    nativeLineItems.Add(new NativeModels.JournalLine());
                }
                else
                {
                    var lineItem = payment.ToNativeDebitLineItem(linenumber);
                    linenumber = lineItem.lineNumber;
                    nativeLineItems.Add(lineItem);
                    lineItem = payment.ToNativeCreditLineItem(linenumber);
                    linenumber = lineItem.lineNumber;
                    nativeLineItems.Add(lineItem);
                }
            }
            return nativeLineItems;
        }

        internal static NativeModels.JournalLine ToNativeDebitLineItem(this PublicModels.Payment payment,int lineNumber)
        {
            return new NativeModels.JournalLine()
            {
                amount = payment.Amount,
                lineNumber = lineNumber + 10000,                
                accountId = "6590a77a-9c7f-eb11-b855-000d3a6a4ff0",
                accountNumber = "47211",
                documentNumber = payment.InvoiceNumber,
                PostingDate = payment.Date,
                dimensionSetLines = new List<NativeModels.DimensionSetLine>() { DimensionSetType.Category.ToDimensionSetLine("ACT") }
            };
        }

        internal static NativeModels.JournalLine ToNativeCreditLineItem(this PublicModels.Payment payment, int lineNumber)
        {
            return new NativeModels.JournalLine()
            {
                amount = payment.Amount * -1,
                lineNumber = lineNumber + 10000,
                accountNumber = "43132",
                documentNumber = payment.InvoiceNumber,
                PostingDate = payment.Date,
                dimensionSetLines = new List<NativeModels.DimensionSetLine>() { DimensionSetType.Category.ToDimensionSetLine("ACT") }
            };
        }

        internal static NativeModels.Payments ToNativePayments(this PublicModels.Payments payments)
        {
            var result = new NativeModels.Payments
            {
                _Payments = new List<NativeModels.Payment>()
            };
            result._Payments = payments._Payments.ToNativePayments();
 
            return result;
        }

        internal static List<NativeModels.Payment> ToNativePayments(this List<PublicModels.Payment> payments)
        {
            var nativePayments = new List<NativeModels.Payment>();

            foreach (var payment in payments)
            {
                if (payment.HasValidationErrors)
                {
                    var baseModel = payment.ToNativeBaseModel();
                    nativePayments.Add(new NativeModels.Payment(baseModel));
                }
                else
                {
                    nativePayments.Add(payment.ToNativePayment());
                }
            }
            return nativePayments;
        }

        internal static NativeModels.Payment ToNativePayment(this PublicModels.Payment payment)
        {
            return new NativeModels.Payment
            {
                Amount = payment.Amount, 
                AppliesToInvoiceNumber = payment.InvoiceNumber, 
                Reference = payment.Reference, 
                CustomerNumber = payment.Account, 
                PostingDate = payment.Date,
                InvoiceId = payment.InvoiceId,
                CreditNoteId = payment.CreditNoteId
            };
        }

        internal static PublicModels.Payments ToPublicPayments(this NativeModels.Payments payments)
        {
            var result = new PublicModels.Payments()
            {
                _Payments = new List<PublicModels.Payment>()
            };
            foreach (var payment in payments._Payments)
            {
                if (payment.HasValidationErrors)
                {
                    var baseModel = payment.ToPublicBaseModel();
                    result._Payments.Add(new PublicModels.Payment(baseModel));
                }
                else
                {
                    result._Payments.Add(payment.ToPublicPayment());
                }
            }
            return result;
        }

        internal static System.Collections.Generic.List<string> ToInvoiceNumbers(this PublicModels.Payments payments)
        {
            return payments._Payments.Select(x => x.InvoiceNumber).ToList();
        }

        internal static PublicModels.Payment ToPublicPayment(this NativeModels.Payment payment)
        {
            var baseModel = payment.ToPublicBaseModel();
            return new PublicModels.Payment
            {
                Amount = payment.Amount,
                InvoiceNumber = payment.Number,
                CreditNoteNumber = payment.Number,                
                Reference = payment.Reference,
                Account = payment.CustomerNumber,
                Date = Convert.ToDateTime(payment.PostingDate),
                HasValidationErrors = baseModel.HasValidationErrors, 
                ValidationErrors = baseModel.ValidationErrors
            };
        }

        internal static PaymentType ToPaymentType(this Guid PaymentTypeId)
        {
            var paymentString = PaymentTypeId.ToString().ToUpper();
            switch (paymentString)
            {
                case "4F7290B0-5081-EB11-B855-000D3A6A4611":
                    return PaymentType.SecurePay;
                case "B252122B-5A80-EB11-B855-000D3A6A4611":
                    return PaymentType.PayPal;
                case "A6E168B2-3D7C-EB11-B854-000D3AD1A7EA":
                    return PaymentType.DirectDeposit;
                default:
                    throw new Exception($"Unknown Payment Type: {paymentString}");
            }
        }

        internal enum PaymentType
        {
            DirectDeposit,
            SecurePay,
            PayPal,
            Cash,
            
        }
    }
}
