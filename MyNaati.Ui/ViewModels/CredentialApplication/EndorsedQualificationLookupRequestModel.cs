namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class EndorsedQualificationLookupRequestModel : LookupsRequestModel
    {
        public string Institution { get; set; }
        public string Location { get; set; }
        public int InstitutionId { get; set; }
    }
}