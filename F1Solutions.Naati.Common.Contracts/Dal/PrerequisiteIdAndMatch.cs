using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts.Models.CredentialPrerequisite
{
    public class PrerequisiteIdAndMatch
    {
        public int Id { get; set; }
        public bool? Match { get; set; }
        public string PrerequisiteCredential { get; set; }
    }
}
