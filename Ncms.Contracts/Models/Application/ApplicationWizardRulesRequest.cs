namespace Ncms.Contracts.Models.Application
{
    public class ApplicationWizardRulesRequest
    {
        public int ApplicationId { get; set; }
        public string ApplicationStatus { get; set; }
        public int ActionId { get; set; }
        public int? CredentialRequestId { get; set; }
    }
}