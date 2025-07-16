using F1Solutions.Naati.Common.Contracts.Dal.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static F1Solutions.Naati.Common.Contracts.Dal.CreatePrerequisiteApplicationsDalModel;

namespace F1Solutions.Naati.Common.Contracts
{
    public class PrerequisiteApplicationDalModel
    {
        public UpsertCredentialApplicationRequest UpsertCredentialApplicationRequest { get; set; }
        public IEnumerable<CredentialApplicationAttachmentDalModel> Attachments { get; set; }
    }
}
