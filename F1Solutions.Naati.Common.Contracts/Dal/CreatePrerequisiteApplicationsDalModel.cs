using F1Solutions.Naati.Common.Contracts.Dal.Request;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CreatePrerequisiteApplicationsDalModel
    {
        public int ApplicationPersonId { get; set; }
        public int ApplicationId { get; set; }
        public IList<ApplicationField> ApplicationFields { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialRequestSkillDirectionId { get; set; }
        public int CredentialRequestSkillId { get; set; }
        public string CredentialRequestSkillLanguage1 { get; set; }
        public string CredentialRequestSkillLanguage2 { get; set; }
        public int CredentialRequestSkillTypeId { get; set; }
        public int ParentApplicationId { get; set; }
        public int ParentCredentialRequestId { get; set; }
        public ChildPreRequisites ChildCredentialRequestType { get; set; }
        public int EnteredUserId { get; set; }
        public int PreferredTestLocationId { get; set; }

        public class ApplicationField
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public int FieldDataId { get; set; }
            public int FieldTypeId { get; set; }
            public string Value { get; set; }
            public int FieldOptionId { get; set; }
        }
    }
}
