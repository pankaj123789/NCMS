namespace Ncms.Contracts.Models
{
    public class LanguageModel
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Indigenous { get; set; }
        public int? GroupLanguageId { get; set; }
    }
}
