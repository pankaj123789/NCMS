namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetCredentialTypeTemplateRequest
    {
        public int CredentialApplicationId { get; set; }
        public int CredentialTypeId { get; set; }
        public string TempFilePath { get; set; }
        public int CredentialId { get; set; }
    }
}