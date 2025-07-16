using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialRequest : EntityBase
    {
        private IList<CredentialCredentialRequest> mCredentialCredentialRequests = new List<CredentialCredentialRequest>();
        private IList<CredentialRequestFieldData> mCredentialRequestFieldsData = new List<CredentialRequestFieldData>();
        private IList<CredentialWorkflowFee> mCredentialWorkflowFees = new List<CredentialWorkflowFee>();
        private IList<TestSitting> mTestSittings = new List<TestSitting>();

        public virtual CredentialApplication CredentialApplication { get; set; }
        public virtual CredentialType CredentialType { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual CredentialRequestStatusType CredentialRequestStatusType { get; set; }
        public virtual DateTime StatusChangeDate { get; set; }
        public virtual User StatusChangeUser { get; set; }
        public virtual IEnumerable<CredentialCredentialRequest> CredentialCredentialRequests => mCredentialCredentialRequests;
        public virtual IEnumerable<CredentialRequestFieldData> CredentialRequestFieldsData => mCredentialRequestFieldsData;
        public virtual IList<CredentialWorkflowFee> CredentialWorkflowFees => mCredentialWorkflowFees;

        public virtual CredentialRequestPathType CredentialRequestPathType { get; set; }
        public virtual bool Supplementary { get; set; }
        public virtual bool? AutoCreated { get; set; }
        public virtual IList<TestSitting> TestSittings => mTestSittings;
        public virtual IList<Credential> Credentials
        {
            get
            {
                return CredentialCredentialRequests?
                    .Select(x => x.Credential)
                    .ToList();
            }
        }

        public override IAuditObject RootAuditObject => CredentialApplication;
    }
}
