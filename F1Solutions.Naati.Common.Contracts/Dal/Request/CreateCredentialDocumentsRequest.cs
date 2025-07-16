using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class CreateCredentialDocumentsRequest
    {
        public int CredentialId { get; set; }
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        public IEnumerable<DocumentData> Documents { get; set; }
    }
}