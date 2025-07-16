namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class InstitutionInsertResponse
    {
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public bool IsSuccessful { get; set; }
        public bool IsWarned { get; set; }
        public int InstitutionId { get; set; }
    }
}