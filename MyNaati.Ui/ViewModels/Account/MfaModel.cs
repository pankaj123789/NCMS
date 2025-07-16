namespace MyNaati.Ui.ViewModels.Account
{
    public class MfaModel
    {
        public string returnUrl { get; set; }
        public string error { get; set; }
        public int naatiNumber { get; set; }
        public bool mfaAlreadyConfigured { get; set; }
        public string code { get; set; }
        public string img { get; set; }
        public string email { get; set; }
        public string output { get; set; }
    }
}