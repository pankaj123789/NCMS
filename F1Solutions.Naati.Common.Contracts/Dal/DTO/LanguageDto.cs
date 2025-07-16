namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class LanguageDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Indigenous { get; set; }
        public int? GroupLanguageId { get; set; }
        public string GroupLanguageName { get; set; }
    }
}