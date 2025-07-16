namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveLanguageRequest
    {
        public int? LanguageId { get; set; }
        public int? GroupLanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int UserId { get; set; }
        public string Note { get; set; }
    }
}