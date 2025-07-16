using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public class WiisePaymentCreationOperation : WiiseOperation<WiiseCreatePaymentRequest, Payments>
    {
        private (string invoiceNumber, string creditNoteNumber) GetReferenceNumber()
        {
            if (!Request.PrerequisiteRequestId.HasValue)
            {
                throw new Exception(
                    "Can't get invoice number to create payment as there is no invoice creation operation registered on this payment creation operation.");
            }
            var queue = new ExternalAccountingQueueService();
            var invoiceReq = queue.GetOperationRequest(Request.PrerequisiteRequestId.Value);

            var isCreditNote = invoiceReq.Type.Id == (int)Contracts.Dal.Enum.ExternalAccountingOperationTypeName.CreateCreditNote;

            if (invoiceReq == null)
            {
                LoggingHelper.LogWarning($"invoice operation {Request.PrerequisiteRequestId.Value} not found for prerequisite");
            }
            else
            {
                var message = isCreditNote ? $"credit note operation {Request.PrerequisiteRequestId.Value} has draft output value of {invoiceReq.Output}" :
                    $"invoice operation {Request.PrerequisiteRequestId.Value} has draft output value of {invoiceReq.Output}";

                LoggingHelper.LogInfo(message);
            }  
            
            return (
                invoiceNumber: isCreditNote ? null : invoiceReq.Output,
                creditNoteNumber: isCreditNote ? invoiceReq.Output : null
            );
        }

        protected override Task PrepareInput()
        {
            if (Request?.Payments == null || !Request.Payments.Any())
            {
                throw new Exception("Null or empty payment creation request.");
            }

            var referenceNumberFromPrerequisite = Request.PrerequisiteRequestId.HasValue ? GetReferenceNumber() : (invoiceNumber: "", creditNoteNumber: "");

            var list = new List<Payment>();
            var payments = new Payments
            {
                _Payments = list
            };

            foreach (var payment in Request.Payments)
            {
                var invoiceNumber = payment.InvoiceNumber ?? referenceNumberFromPrerequisite.invoiceNumber;
                var creditNoteNumber = payment.CreditNoteNumber ?? referenceNumberFromPrerequisite.creditNoteNumber;

                if (invoiceNumber == null && creditNoteNumber == null)
                {
                    throw new Exception($"Cannot create payment {payment.Reference}: no invoice or credit note number.");
                }

                list.Add(new Payment
                {
                    Account = payment.AccountId.ToString(),
                    InvoiceNumber = invoiceNumber,
                    CreditNoteNumber = creditNoteNumber,
                    Amount = Decimal.ToDouble(payment.Amount),
                    Date = new DateTime(payment.Date.ToLocalTime().Date.Ticks, DateTimeKind.Unspecified),
                    Reference = payment.Reference,
                    //Status = Payment.StatusEnum.AUTHORISED,
                    //PaymentType = Payment.PaymentTypeEnum.ACCRECPAYMENT
                });
            }

            Input = payments;

            return Task.CompletedTask;
        }

        protected void LogPayments(IEnumerable<Payment> payments)
        {
            foreach (var payment in payments)
            {
                try
                {
                    LoggingHelper.LogInfo("Wiise Payment {PaymentReference} created; Amount: {PaymentAmount}, Invoice: {InvoiceNumber}",
                        payment.Reference, payment.Amount, payment.InvoiceNumber);
                }
                catch { }
            }
        }

        protected override async Task<Payments> ProtectedPerformOperation()
        {
            var journalnumber = WiiseExtensions.WiiseJournalNumber();
            var result = await Wiise.CreatePaymentsAsync(Token.Value, Token.Tenant, Input, journalnumber);
            LogPayments(result.Data._Payments);
            if(result.StatusCode.IsStatusCodeAnError())
            {
                LoggingHelper.LogError($"Payment for {Input._Payments.First().InvoiceNumber} returned an error");
                throw new Exception($"Payment for {Input._Payments.First().InvoiceNumber} returned an error");
            }
            return result.Data;
        }

        protected override void PrepareOutput()
        {
            if (ProtectedResult != null && ProtectedResult._Payments.Any())
            {
                Output = Request.Reference;
            }
        }
    }

    public class WiiseCreatePaymentRequest : WiiseOperationRequest
    {
        public IEnumerable<WiiseCreatePaymentModel> Payments { get; set; }

        public string Reference
        {
            get
            {
                if (Payments == null || !Payments.Any())
                {
                    return String.Empty;
                }
                return String.Join("; ", Payments.Select(x => $"[{x.InvoiceNumber ?? x.CreditNoteNumber}::{x.Reference}]"));
            }
        }
    }

    public class WiiseCreatePaymentModel
    {
        public string InvoiceNumber { get; set; }
        public string CreditNoteNumber { get; set; }
        public string Reference { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}

