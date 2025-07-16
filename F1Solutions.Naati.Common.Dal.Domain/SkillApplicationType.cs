using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SkillApplicationType : EntityBase
    {
        public virtual Skill Skill { get; set; }
        public virtual CredentialApplicationType CredentialApplicationType { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
    }
}
