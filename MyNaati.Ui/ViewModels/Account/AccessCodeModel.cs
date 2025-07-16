namespace MyNaati.Ui.ViewModels.Account
{
    public class AccessCodeModel
    {
        public string returnView { get; set; }
        public string returnController { get; set; }
        public string error { get; set; }
        public string code { get; set; }
        public bool codeSent { get; set; }
    }
}