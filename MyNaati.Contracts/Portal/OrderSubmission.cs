using System;
using System.Runtime.Serialization;

namespace MyNaati.Contracts.Portal
{
    
    public class OrderDetailsRequest
    {
        
        public int ReferenceNumber { get; set; }
    }

    //
    //public class OrderDetailsDto
    //{
    //    
    //    public DateTime Date { get; set; }

    //    
    //    public string DeliveryName { get; set; }

    //    
    //    public string DeliveryAddress { get; set; }

    //    
    //    public int SuburbId { get; set; }

    //    
    //    public int CountryId { get; set; }

    //    
    //    public List<OrderItemDetailsDto> OrderItems { get; set; } 
    //}

    //
    //public class OrderItemDetailsDto
    //{
    //    
    //    public string Product { get; set; }

    //    
    //    public string Skill { get; set; }

    //    
    //    public string Level { get; set; }

    //    
    //    public string Direction { get; set; }

    //    
    //    public DateTime? Expiry { get; set; }

    //    
    //    public int Quantity { get; set; }

    //    
    //    public decimal Price { get; set; }

    //    
    //    public bool GSTApplies { get; set; }

    //    
    //    public OrderItemType OrderItemType { get; set; }

    //    
    //    public int ProductSpecificationId { get; set; }

    //    
    //    public int? AccreditationResultId { get; set; }
    //}

    //
    //public class OrderDetailsResponse
    //{
    //    
    //    public int NaatiNumber { get; set; }

    //    
    //    public OrderDetailsDto Order { get; set; }
    //}

    //
    //public class UnpaidOrderSaveRequest
    //{
    //    
    //    public string ReferenceNumber { get; set; }

    //    
    //    public Order Order { get; set; }
    //}

    
    public class UnpaidOrderSaveResponse
    {
        
        public bool Success { get; set; }
    }

    //
    //public class OrderSubmissionRequest
    //{
    //    
    //    public string ReferenceNumber { get; set; }

    //    
    //    public Order Order { get; set; }

    //    
    //    public EnteredCardDetails CardDetails { get; set; }

    //    
    //    public PractitionerDirectoryUpdateListingRequest PDListingUpdate { get; set; }
    //}

    
    public class PaymentRequest
    {
        
        public string InvoiceNumber { get; set; }

        
        public string ApplicationId { get; set; }

        
        public Decimal Amount { get; set; }

        
        public string NAATINumber { get; set; }

        
        public DateTime PaymentDate { get; set; }

        
        public EnteredCardDetails CardDetails { get; set; }
    }


    
    public enum PaymentProcessStatus
    {
        [EnumMember]
        NotAttempted,

        [EnumMember]
        Failed,

        [EnumMember]
        Succeeded
    }

    
    public class OrderSubmissionResponse
    {
        
        public bool Success { get; set; }

        
        public PaymentProcessStatus PaymentStatus { get; set; }
        
        
        public PaymentFailureDetails PaymentFailureDetails { get; set; }

        
        public ReturnedCardDetails CardDetailsForReceipt { get; set; }

        
        public int OrderId { get; set; }
    }

    
    public class PaymentFailureDetails
    {
        
        public bool SystemError { get; set; }

        
        public string SystemErrorMessage { get; set; }

        
        public bool RejectedPayment { get; set; }

        
        public string RejectionCode { get; set; }

        
        public string RejectionDescription { get; set; }
    }
    
    public class EnteredCardDetails
    {
        
        public string NameOnCard { get; set; }

        
        public CardType Type { get; set; }

        
        public string CardNumber { get; set; }

        
        public DateTime ExpiryMonth { get; set; }

        
        public string CardVerificationValue { get; set; }
        public string CardToken { get; set; }
    }

    
    public class ReturnedCardDetails
    {
        
        public string CardNumber { get; set; }

        
        public string ExpiryMonth { get; set; }

        
        public CardType Type { get; set; }
    }


    
    public class PaymentResponse
    {
        
        public bool Success { get; set; }

        
        public PaymentProcessStatus PaymentStatus { get; set; }

        
        public PaymentFailureDetails PaymentFailureDetails { get; set; }

        
        public ReturnedCardDetails CardDetailsForReceipt { get; set; }

        
        public string InvoiceNumber { get; set; }

        
        public string ApplicationId { get; set; }

        
        public string ReferenceNumber { get; set; }

        
        public bool UnHandledException { get; set; }
        
        public string UnHandledExceptionMessage { get; set; }

        public string OrderNumber { get; set; }
    }

    
    public enum CardType
    {
        [EnumMember]
        Unknown,

        [EnumMember]
        JCB,

        [EnumMember]
        Amex,

        [EnumMember]
        DinersClub,

        [EnumMember]
        Bankcard,

        [EnumMember]
        MasterCard,

        [EnumMember]
        Visa
    }

}
