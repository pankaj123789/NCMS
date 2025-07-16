namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class PayOnlineResponseModel
    {
        public bool IsSecurePayStatusSuccess { get; set; }
        public bool IsSecurePayResponseSuccess { get; set; }
        public bool IsPayPalStatusSuccess { get; set; }
        public bool IsPayPalResponseSuccess { get; set; }
        public bool IsException { get; set; }

        public bool IsWiiseInvoiceSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public string SecurePayReference { get; set; }
        public string PayPalReference { get; set; }

        public string WiiseInvoice { get; set; }

        public int? FeesQuestionId { get; set; }
        
        public bool IsPayByCreditCard { get; set; }
        public bool IsPayByPayPal { get; set; }

        public string OrderNumber { get; set; }
    }
}