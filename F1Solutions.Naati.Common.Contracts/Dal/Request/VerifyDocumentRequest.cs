using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class VerifyDocumentRequest
    {
        public int CredentialId { get; set; }
        public int CredentialApplicationId { get; set; }
    }
}
