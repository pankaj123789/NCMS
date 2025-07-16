using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CreateInvoiceResponse : CreatePayableResponse
    {
        public string InvoiceNumber => Number;
        public Guid InvoiceId => Id;
        public string InvoiceErrorMessage => ErrorMessage;
        public string InvoiceWarningMessage => WarningMessage;
    }
}