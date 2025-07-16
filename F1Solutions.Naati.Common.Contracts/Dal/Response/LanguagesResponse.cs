using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class LanguagesResponse :BaseResponse
    {
        public IEnumerable<ApiLanguage> Results { get; set; }
    }

    public class ApiLanguage
    {
        public string DisplayName { get; set; }
        public IEnumerable<int> SkillIds { get; set; }
    }

    public class ApiSkill
    {
        public string DisplayName { get; set; }
        public int SkillId { get; set; }
    }
}
