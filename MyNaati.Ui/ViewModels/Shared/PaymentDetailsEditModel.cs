using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MyNaati.Contracts.Portal;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class PaymentDetailsEditModel
    {
        // Credit Card Details.  Payment provider is Securepay, via the Secure XML API.  
        // Validation rules must comply with http://www.securepay.com.au/resources/Secure_XML_API_Integration_Guide.pdf
        // pages 16-18.  See especially section 6.1.1.1.2 CreditCardInfo Element (pg18).
        // Due to NAATI reqs, we're actually collecting more details than necessary (i.e. NameOnCard and Type)

        public PaymentDetailsEditModel()
        {
            ExpiryMonths = Enumerable.Range(1, 12)
                .Select(month => new SelectListItem() { Text = month.ToString().PadLeft(2, '0'), Value = month.ToString() })
                .ToList();

            // Allow expiry years up to 10 years into the future.
            ExpiryYears = Enumerable.Range(DateTime.Now.Year, 11)
                .Select(year => new SelectListItem() { Text = year.ToString(), Value = year.ToString() })
                .ToList();
        }

        [Required]
        [DisplayName("Card type")]
        public CardType? Type { get; set; }

        [Required]
        [DisplayName("Name on card")]
        public string NameOnCard { get; set; }

        [Required]
        [RegularExpression("^[0-9]{13,16}$", ErrorMessage = "Card number must be between 13 and 16 digits.")]
        [DisplayName("Card number")]
        [UIHint("creditCardNumber")]
        public string CardNumber { get; set; }
        
        // Field name is "CSC" in the spec.
        [Required]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = "CSC must be 3 digits.")]
        [DisplayName("CSC")]
        public string CVV { get; set; }

        [Required]
        [DisplayName("Expiration date")]
        [UIHint("listToSelect")]
        [MonthValidator]
        public int ExpiryMonth { get; set; }

        [Required]
        [UIHint("listToSelect")]
        public int ExpiryYear { get; set; }

        public IList<SelectListItem> ExpiryMonths { get; set; }

        public IList<SelectListItem> ExpiryYears { get; set; }

        public class MonthValidator : ValidationAttribute
        {
            private const string mErrorMessage = "Expiry date cannot be in the past.";

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PaymentDetailsEditModel) validationContext.ObjectInstance;

                if (model.ExpiryYear == DateTime.Now.Year && model.ExpiryMonth < DateTime.Now.Month)
                {
                    return new ValidationResult(mErrorMessage, new[] { "ExpiryMonth" });
                }

                return null;
            }
        }
    }
}