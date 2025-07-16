namespace MyNaati.Ui.ViewModels.Account
{
    public class MfaAndAccessCodeModel
    {
        /// <summary>
        /// This means either an MFA code has been enbtered or email activations has occurred
        /// </summary>
        public bool MfaActive { get; set; }
        /// <summary>
        /// This means that an MFA code has been entered. It doesnt take into account an email code being entered
        /// </summary>
        public bool MfaConfigured { get; set; }
        public string MfaCode { get; set; }
        public string ReturnView { get; set; }
        public string ReturnController { get; set; }
        public string Error { get; set; }
        public string Email{ get; set; }
    }
}