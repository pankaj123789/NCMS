using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class CreateCredentialCertificateResponse
    {
        public IEnumerable<int> StoredFileIds { get; set; }
    }
}