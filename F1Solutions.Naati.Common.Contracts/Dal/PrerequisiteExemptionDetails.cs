using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CredentialRequestDetails
    {
        public int PersonId { get; set; }
        public int CredentialTypeId { get; set; }
        public int SkillId { get; set; }
        public int ApplicationTypeId { get; set; }
    }

    public class CredentialPrerequisiteDetails
    {
        public int CredentialPrerequisiteId { get; set; }
        public string CredentialPrerequsiteName { get; set; }
        public int CredentialTypePrerequisiteId { get; set; }
        public int ApplicationTypePrerequisiteId { get; set; }
        public string SkillName { get; set; }
        public int SkillId { get; set; }
    }

    public class SkillDetails
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
    }
}
