using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MyNaati.Contracts.Portal;

namespace MyNaati.Ui.ViewModels.Bills
{
    public class PaymentDetailsModel
    {
        // Credit Card Details.  Payment provider is Securepay, via the Secure XML API.  
        // Validation rules must comply with http://www.securepay.com.au/resources/Secure_XML_API_Integration_Guide.pdf
        // pages 16-18.  See especially section 6.1.1.1.2 CreditCardInfo Element (pg18).
        // Due to NAATI reqs, we're actually collecting more details than necessary (i.e. NameOnCard and Type)

        //public PaymentDetailsModel()
        //{
        //    ExpiryMonths = Enumerable.Range(1, 12)
        //        .Select(month => new SelectListItem() { Text = month.ToString().PadLeft(2, '0'), Value = month.ToString() })
        //        .ToList();

        //    // Allow expiry years up to 10 years into the future.
        //    ExpiryYears = Enumerable.Range(DateTime.Now.Year, 11)
        //        .Select(year => new SelectListItem() { Text = year.ToString(), Value = year.ToString() })
        //        .ToList();
        //}

        public bool IsPaymentSuccessful { get; set; }
        public bool IsWiiseSuccessful { get; set; }

        public bool IsPaidWithError { get; set; }
        public string InvoiceNumber { get; set; }
        public Guid InvoiceId { get; set; }
        public string NaatiNumber { get; set; }

        public string BillType { get; set; }

        public string OnlineOfficeAbbr { get; set; }
        public string EFTMachineTerminal { get; set; }

        [DisplayName("Paid Amount")]
        public decimal Payments { get; set; }

        [DisplayName("Invoice Amount")]
        public decimal InvoiceAmount { get; set; }

        [DisplayName("Amount Due")]
        public decimal AmountDue { get; set; }

        [Required]
        [RegularExpression("^[0-9]+(\\.[0-9]{1,2})?$", ErrorMessage = "Amount must be up to two decimal point.")]
        [DisplayName("Amount to pay")]
        [AmountValidator]
        public decimal AmountPay { get; set; }

        public CardType? Type { get; set; }

        //[Required]
        //[DisplayName("Name on card")]
        //public string NameOnCard { get; set; }

        //[Required]
        //[RegularExpression("^[0-9]{13,16}$", ErrorMessage = "Card number must be between 13 and 16 digits.")]
        //[DisplayName("Card number")]
        //[UIHint("creditCardNumber")]
        //public string CardNumber { get; set; }

        // Field name is "CSC" in the spec.
        //[Required]
        //[RegularExpression("^[0-9]{3}$", ErrorMessage = "CSC must be 3 digits.")]
        //[DisplayName("CSC")]
        //public string CVV { get; set; }

        //[Required]
        //[DisplayName("Expiration date")]
        //[UIHint("listToSelect")]
        //[MonthValidator]
        //public int ExpiryMonth { get; set; }

        //[Required]
        //[UIHint("listToSelect")]
        //public int ExpiryYear { get; set; }

        public bool AllowPaymentByVisa { get; set; }
        public bool AllowPaymentByMasterCard { get; set; }
        public bool AllowPaymentByAmex { get; set; }
        public bool AllowPaymentByDinersClub { get; set; }
        public bool AllowPaymentByJcb { get; set; }
        public bool AllowPaymentByPayPal { get; set; }


        //public IList<SelectListItem> ExpiryMonths { get; set; }

        //public IList<SelectListItem> ExpiryYears { get; set; }

        [Required]
        [DisplayName("Please check the credit card details")]
        public string CreditCardToken { get; set; } 

        public string LastDigits { get; set; }

        //public class MonthValidator : ValidationAttribute
        //{
        //    private const string mErrorMessage = "Expiry date cannot be in the past.";

        //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //    {
        //        var model = (PaymentDetailsModel)validationContext.ObjectInstance;

        //        if (model.ExpiryYear == DateTime.Now.Year && model.ExpiryMonth < DateTime.Now.Month)
        //        {
        //            return new ValidationResult(mErrorMessage, new[] { "ExpiryMonth" });
        //        }

        //        return null;
        //    }
        //}

        public class AmountValidator : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PaymentDetailsModel)validationContext.ObjectInstance;

                if (model.AmountPay > model.AmountDue)
                {
                    return new ValidationResult("Amount cannot be greater than amount due.", new[] { "AmountPay" });
                }
                if (model.AmountPay == 0)
                {
                    return new ValidationResult("Amount cannot be zero.", new[] { "AmountPay" });
                }
                return null;
            }
        }

    }
}