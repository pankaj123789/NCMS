using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CertificationPeriod : EntityBase
    {
        private IEnumerable<Credential> mCredentials = Enumerable.Empty<Credential>();
        private IEnumerable<Recertification> mRecertifications = Enumerable.Empty<Recertification>();

        public virtual Person Person { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual DateTime OriginalEndDate { get; set; }

        public virtual IEnumerable<Credential> Credentials => mCredentials;
        public virtual IEnumerable<Recertification> Recertifications => mRecertifications;
        public virtual CredentialApplication CredentialApplication { get; set; }
    }
}
