namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class LanguageSearchResultDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? GroupLanguageId { get; set; }
        public string GroupLanguageName { get; set; }
        public string Note { get; set; }
    }
}