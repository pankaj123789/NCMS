using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Credential : EntityBase
    {
        private IList<CredentialQrCode> mCredentialQrCodes;
        private IList<CredentialCredentialRequest> mCredentialCredentialRequests = new List<CredentialCredentialRequest>();
        private IList<CredentialAttachment> mCredentialAttachments = new List<CredentialAttachment>();
        private IList<WorkPractice> mWorkPractices = new List<WorkPractice>();

        public virtual  DateTime StartDate { get; set; }
        public  virtual  DateTime? ExpiryDate { get; set; }
        public  virtual  DateTime? TerminationDate { get; set; }
        public  virtual bool ShowInOnlineDirectory { get; set; }
        public virtual CertificationPeriod CertificationPeriod { get; set; }
        public  virtual IEnumerable<CredentialCredentialRequest> CredentialCredentialRequests => mCredentialCredentialRequests;
        public virtual IEnumerable<CredentialAttachment> CredentialAttachments => mCredentialAttachments;
        public virtual IEnumerable<WorkPractice> WorkPractices => mWorkPractices;
        public virtual IEnumerable<CredentialQrCode> CredentialQrCodes => mCredentialQrCodes;
        public virtual void AddQrCode(CredentialQrCode credentialQrCode)
        {
            credentialQrCode.Credential = this;
            mCredentialQrCodes.Add(credentialQrCode);
        }
    }
}
