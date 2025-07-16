namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class DeleteRequestModel
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }

        public int DocumentId { get; set; }

        public int Token { get; set; }
    }
}