using MyNaati.Contracts.BackOffice;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class SubmitApplicationResponse
    {
        public SaveApplicationFormResponse SaveApplicationFormResponse { get; set; }
        public PayOnlineResponseModel PayOnlineResponse { get; set; }
        public string ConfirmContent { get; set; }
    }
}