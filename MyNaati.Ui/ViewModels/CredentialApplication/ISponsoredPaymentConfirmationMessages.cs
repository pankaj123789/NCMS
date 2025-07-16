namespace MyNaati.Ui.ViewModels.CredentialApplication
{
	interface ISponsoredPaymentConfirmationMessages
	{
		string TrustedPayer { get; }
		string NonTrustedPayer { get; }
		string CreditCard { get; }
		string PayPal { get; }
		string CashDirectDeposit { get; }
		string Error { get; }
	}
}
