using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialPrerequisite : EntityBase
    {
        public virtual CredentialType CredentialType { get; set; }
        public virtual CredentialType CredentialTypePrerequisite { get; set; }
        public virtual CredentialApplicationType CredentialApplicationType { get;set;}
        public virtual CredentialApplicationType ApplicationTypePrerequisite { get; set; }
        public virtual bool SkillMatch { get; set; }
    }
}
