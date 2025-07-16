using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class PrerequisiteApplicationsDalModel
    {
        public string CurrentCredentialRequest { get; set; }
        public string RequiredPrerequisite { get; set; }
        public int ExistingApplicationId { get; set; }
        public string ExistingApplication { get; set; }
        public int ExistingApplicationStatusTypeId { get; set; }
        public string ExistingApplicationStatus { get; set; }
        public bool? ExistingApplicationAutoCreated { get; set; }
        public string ExistingCredentialRequest { get; set; }
        public int ExistingCredentialRequestStatusTypeId { get; set; }
        public string ExistingCredentialRequestStatus { get; set; }
        public bool? ExistingCredentialRequestAutoCreated { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ExistingApplicationType { get; set; }
    }
}
