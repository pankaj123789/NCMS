using F1Solutions.Naati.Common.Contracts.Security;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class UserPermissionsDto
    {
        public SecurityNounName Noun { get; set; }
        public string NounName { get; set; }
        public string NounDisplayName { get; set; }
        public long VerbMask { get; set; }
    }
}