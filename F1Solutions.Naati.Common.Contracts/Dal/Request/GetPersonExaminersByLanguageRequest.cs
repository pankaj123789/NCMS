namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetPersonExaminersByLanguageRequest
    {
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }
        public int CredentialTypeId { get; set; }
    }
}