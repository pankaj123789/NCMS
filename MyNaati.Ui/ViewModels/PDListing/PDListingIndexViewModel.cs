using System;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.PDListing
{
    public class PDListingIndexViewModel
    {
        public decimal AustraliaPrice { get; set; }
        public decimal OverseasPrice { get; set; }

        public bool CanRenew { get; set; }
        public bool IsListedForCurrentFinancialYear { get; set; }
        public bool IsListedForNextFinancialYear { get; set; }

        public bool HasCredentials { get; set; }

        public bool IsContactDetailsRegistered { get; set; }

        public bool PaymentRequired { get; set; }

        [UIHint("dateOnly")]
        public DateTime? CurrentExpiry { get; set; }
        [UIHint("dateOnly")]
        public DateTime? NewExpiry { get; set; }
    }
}