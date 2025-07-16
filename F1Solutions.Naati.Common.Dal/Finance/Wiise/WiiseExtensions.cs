using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using static F1Solutions.Naati.Common.Wiise.PublicModels.Invoice;
using WiiseCreditNoteType = F1Solutions.Naati.Common.Wiise.PublicModels.CreditNote.TypeEnum;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public static class WiiseExtensions
    {
        public static TypeEnum ToWiiseInvoiceAccountType(this Contracts.Dal.Enum.InvoiceType invoiceType)
        {
            switch (invoiceType)
            {
                case Contracts.Dal.Enum.InvoiceType.Invoice:
                    return TypeEnum.ACCREC;
                case Contracts.Dal.Enum.InvoiceType.CreditNote:
                    throw new InvalidCastException("Cannot convert SAM Invoice Type of \"Credit Note\" to Wiise Invoice Type");
                case Contracts.Dal.Enum.InvoiceType.Bill:
                    return TypeEnum.ACCPAY;
                default:
                    throw new ArgumentOutOfRangeException(nameof(invoiceType), invoiceType, null);
            }
        }

        public static InvoiceStatus ToInternalStatus(this Invoice.StatusEnum? statusEnum)
        {
            switch(statusEnum)
            {
                case Invoice.StatusEnum.DRAFT:
                    return InvoiceStatus.Draft;
                case Invoice.StatusEnum.OPEN:
                    return InvoiceStatus.Open;
                case Invoice.StatusEnum.PAID:
                    return InvoiceStatus.Paid;
                case Invoice.StatusEnum.CANCELED:
                    return InvoiceStatus.Canceled;
                default:
                    throw new Exception($"{statusEnum.ToString()} not recognised");
            }
        }

        public static InvoiceStatus ToInternalStatus(this CreditNote.StatusEnum statusEnum)
        {
            switch (statusEnum)
            {
                case CreditNote.StatusEnum.DRAFT:
                    return InvoiceStatus.Draft;
                case CreditNote.StatusEnum.OPEN:
                    return InvoiceStatus.Open;
                case CreditNote.StatusEnum.PAID:
                    return InvoiceStatus.Paid;
                case CreditNote.StatusEnum.CANCELED:
                    return InvoiceStatus.Canceled;
                default:
                    throw new Exception($"{statusEnum.ToString()} not recognised");
            }
        }


        /// <summary>
        /// Done to avoid having to record in increment a number. 
        /// Each Journal number is unique to a tenth of a second for the next 20 years
        /// Has to be no longer than 10 chars
        /// </summary>
        /// <returns></returns>
        public static string WiiseJournalNumber()
        {
            var time = DateTime.Now;
            var alphas = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            var days = (time - DateTime.Parse("2021-01-01")).Days.ToString();
            var hours = alphas[time.Hour];
            var theRest = time.ToString("mmssf");
            return($"{days}{hours}{theRest}");
        }

        public static WiiseCreditNoteType ToWiiseCreditNoteType(this Contracts.Dal.Enum.InvoiceType invoiceType)
        {
            switch (invoiceType)
            {
                case Contracts.Dal.Enum.InvoiceType.CreditNote:
                    return WiiseCreditNoteType.ACCRECCREDIT;
                default:
                    throw new ArgumentOutOfRangeException(nameof(invoiceType), invoiceType, null);
            }
        }

        public static WiiseInvoiceReference GetWiiseReference(this Invoice invoice, IEnumerable<Office> offices = null)
        {
            return new WiiseInvoiceReference(invoice.Reference, offices ?? NHibernateSession.Current.Query<Office>());
        }

        public static WiiseInvoiceReference GetWiiseReference(this CreditNote creditNote, IEnumerable<Office> offices = null)
        {
            return new WiiseInvoiceReference(creditNote.Reference, offices ?? NHibernateSession.Current.Query<Office>());
        }

        public static WiisePaymentReference GetWiiseReference(this Payment payment, string wiisePaymentAccount, IEnumerable<Office> offices = null)
        {
            return new WiisePaymentReference(payment, offices ?? NHibernateSession.Current.Query<Office>(), wiisePaymentAccount);
        }

        public static WiisePaymentReference GetWiiseReference(this CreatePaymentModel payment, Office office, PaymentTypeDto paymentType)
        {
            if (paymentType == PaymentTypeDto.PayPal)
            {
                return new WiisePaymentReference(office, payment.OrderNumber);
            }
            return !string.IsNullOrEmpty(payment.EftMachine)
                ? new WiisePaymentReference(office, payment.EftMachine, paymentType == PaymentTypeDto.AMEX)
                : !string.IsNullOrEmpty(payment.ChequeNumber)
                    ? new WiisePaymentReference(office, payment.BSB, payment.ChequeNumber, payment.ChequeBankName)
                    : new WiisePaymentReference(office);
        }

        public static bool IsRateExceededException(this ApiException ex)
        {
            return ex.ErrorCode == (int)WiiseHttpExceptionResult.TooManyRequests;
        }

    }
}
