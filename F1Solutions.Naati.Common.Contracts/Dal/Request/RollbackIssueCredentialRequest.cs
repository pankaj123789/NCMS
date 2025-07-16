using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class RollbackIssueCredentialRequest
    {
        public int ApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public int ApplicationOriginalStatusId { get; set; }
        public DateTime ApplicationOriginalStatusDate { get; set; }
        public int ApplicationOriginalStatusUserId { get; set; }
        public int CredentialRequestOriginalStatusId { get; set; }
        public DateTime CredentialRequestOriginalStatusDate { get; set; }
        public int CredentialRequestOriginalStatusUserId { get; set; }
        public CredentialDto Credential { get; set; }
        public IEnumerable<int> StoredFileIds { get; set; }
    }
}