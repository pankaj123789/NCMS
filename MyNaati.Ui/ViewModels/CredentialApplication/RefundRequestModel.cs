using F1Solutions.Naati.Common.Contracts.Bl.Attributes;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class RefundRequestModel
    {
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        [LegalCharacters('\r', '\n')]
        public string Comments { get; set; }
        public bool IsManualRefund { get; set; }
        [LegalCharacters('\r', '\n')]
        public string BankAccountDetails { get; set; }
    }
}