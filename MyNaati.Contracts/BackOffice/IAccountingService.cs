using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.BackOffice
{
    
    public interface IAccountingService : IInterceptableservice
    {

        
        GetAccountingInvoicesResponse GetInvoices(GetAccountingInvoiceRequestContract request);

        GetAccountingInvoicesResponse GetUnraisedInvoices(int naatiNumber);

        GetAccountingInvoicePdfResponse GetInvoicePdfByInvoiceId(GetInvoicePdfRequestContract request);

        
        GetAccountingInvoicePdfResponse GetInvoicePdfByInvoiceNumber(GetInvoicePdfRequestContract request);

        
        PaymentCreateResponseModel CreatePayment(PaymentCreateRequestModel paymentCreateRequestModel);

        
        GetOfficeAbbrAndEftMachineTermianlResponse GetOnlineofficeAbbrAndEftMachineTerminal(GetOfficeAbbrAndEftMachineTermianlRequest request);

        
        string GetInvoiceNumber(Guid invoiceId);

    }

    public class GetAccountingInvoicePdfResponse
    {
        public byte[] AccountingInvoicePdfFileContent { get; set; }
    }


    public class GetAccountingInvoicesResponse : AccountingFinanceServiceResponse
    {
        public IEnumerable<AccountingInvoiceDto> Data { get; set; }
        public bool UnHandledException { get; set; }
        public string UnHandledExceptionMessage { get; set; }

    }

    public class GetInvoicePdfRequestContract
    {
        public string InvoiceNumber { get; set; }
        public Guid InvoiceId { get; set; }
        public InvoiceType Type { get; set; }
        public FinanceInfoLocation Location { get; set; }
    }

    public class AccountingFinanceServiceResponse
    {
        public AccountingFinanceServiceResponse()
        {
            Success = true;
            Messages = new List<string>();
            Warnings = new List<string>();
            Errors = new List<string>();
        }

        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Errors { get; set; }
    }
   

    public class AccountingInvoiceDto
    {
        public bool IsNaatiSponsored { get; set; }
        public Guid InvoiceId { get; set; }
        public int? OfficeId { get; set; }
        public string Office { get; set; }
        public int? TransactionId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Total { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal Payment { get; set; }
        public decimal Balance { get; set; }
        public int? NaatiNumber { get; set; }
        public string Customer { get; set; }
        public InvoiceStatus Status { get; set; }
        public InvoiceType Type { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? Date { get; set; }
        public string WiiseReference { get; set; }
        public Guid? ThemeId { get; set; }
        public PaymentDto[] Payments { get; set; }
        public int? CredentialApplicationTypeId { get; set; }
        public string CredentialApplicationTypeDisplayName { get; set; }
        public int? CredentialApplicationId { get; set; }
        public bool? GST { get; set; }
        public string Code { get; set; }
    }



    public class GetAccountingInvoiceRequestContract : AccountingEndOfPeriodRequest
    {
        
        public bool IncludeFullPaymentInfo { get; set; } = true;
        
        public bool ExcludePayables { get; set; } = true;
    }

    
    public class GetOfficeAbbrAndEftMachineTermianlResponse
    {
        
        public string OnlineOfficeAbbr { get; set; }
        
        public string OnlineEftMachineTerminalNo { get; set; }
    }

    
    public class GetOfficeAbbrAndEftMachineTermianlRequest
    {
        
        public int OnlineOfficeId { get; set; }
        
        public int OnlineEftMachineId { get; set; }
    }

    
    public class AccountingEndOfPeriodRequest
    {
        
        public int[] NaatiNumber { get; set; }
        
        public DateTime? DateCreatedFrom { get; set; }
        
        public DateTime? DateCreatedTo { get; set; }
        
        public int[] Office { get; set; }
        
        public int[] EftMachine { get; set; }
        
        public string[] InvoiceNumber { get; set; }
        
        public string[] PaidToAccount { get; set; }
        
        public string[] PaymentType { get; set; }
    }

    public enum InvoiceStatus
    {
        Draft,
        Open,
        Paid,
        Voided
    }

    public enum InvoiceType
    {
        Invoice = 1,
        CreditNote = 2,
        Bill = 3
    }

    public class PaymentDto
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public int? OfficeId { get; set; }
        public string Office { get; set; }
        public int? NaatiNumber { get; set; }
        public string Customer { get; set; }
        public DateTime DatePaid { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentType { get; set; }
        public string BSB { get; set; }
        public string ChequeNumber { get; set; }
        public string BankName { get; set; }
        public int? EftMachineId { get; set; }
        public string EftMachine { get; set; }
        public string Reference { get; set; }
        public string PaymentAccount { get; set; }
    }
    
    public enum FinanceInfoLocation
    {
        Sam = 1,
        Xero = 2,
        Wiise = 3
    }


    public class PaymentCreateRequestModel
    {
        public string InvoiceNumber { get; set; }
        public int OfficeId { get; set; }
        public PaymentTypeDto PaymentType { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
        public Guid AccountId { get; set; }
        public string Reference { get; set; }
        public string UserName { get; set; }
        public string NaatiNumber { get; set; }

    }

    public class PaymentCreateResponseModel
    {
        public string InvoiceId { get; set; }
        public string Reference { get; set; }
        public int? OperationId { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public string PaymentId { get; set; }
        public string UnHandledExceptionMessage { get; set; }
        public bool UnHandledException { get; set; }
    }

    public enum PaymentTypeDto
    {
        Cash = 1,
        Cheque = 2,
        Eft = 3,
        AMEX = 4,
        PayPal = 5
    }
}
