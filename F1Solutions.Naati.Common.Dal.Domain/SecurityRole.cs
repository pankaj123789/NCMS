using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SecurityRole : EntityBase
    {
        private IList<SecurityRule> mSecurityRules = new List<SecurityRule>();

        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool System { get; set; }
 
        public virtual IEnumerable<SecurityRule> SecurityRules => mSecurityRules;

        public override string ToString()
        {
            return Description;
        }
    }
}
