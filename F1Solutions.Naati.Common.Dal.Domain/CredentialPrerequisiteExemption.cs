using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialPrerequisiteExemption : EntityBase
    {
        public virtual Person Person { get; set; }
        public virtual CredentialType CredentialType { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual User ModifiedUser { get; set; }
    }
}
