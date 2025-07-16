using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Skill : EntityBase
    {
		private IList<CredentialRequest> mCredentialRequests;
		private IList<SkillApplicationType> mSkillApplicationTypes;

        public virtual SkillType SkillType { get; set; }
		public virtual Language Language1 { get; set; }
        public virtual Language Language2 { get; set; }
		public virtual string DisplayName => DirectionType.DisplayName
            .Replace("[Language 1]", Language1.Name)
            .Replace("[Language 2]", Language2.Name);
		public virtual DirectionType DirectionType { get; set; }
		public virtual IEnumerable<CredentialRequest> CredentialRequests => mCredentialRequests;
		public virtual IEnumerable<SkillApplicationType> SkillApplicationTypes => mSkillApplicationTypes;
		public virtual User ModifiedUser { get; set; }
		public virtual bool ModifiedByNaati { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
        public virtual string Note { get; set; }

        public virtual void AddSkillApplicationType(SkillApplicationType skillApplicationType)
        {
            skillApplicationType.Skill = this;
            mSkillApplicationTypes.Add(skillApplicationType);
        }

        public virtual void RemoveSkillApplicationType(SkillApplicationType skillApplicationType)
        {
            var result = (from pn in mSkillApplicationTypes
                          where pn.Id == skillApplicationType.Id
                          select pn).SingleOrDefault();

            if (result != null)
            {
                mSkillApplicationTypes.Remove(result);
                skillApplicationType.Skill = null;
            }
        }
        public Skill()
        {
            mSkillApplicationTypes = new List<SkillApplicationType>();
            mCredentialRequests = new List<CredentialRequest>();
        }
    }
}
