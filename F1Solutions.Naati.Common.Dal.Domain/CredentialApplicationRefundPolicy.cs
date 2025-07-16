using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialApplicationRefundPolicy : EntityBase
    {
        private IList<RefundPolicyParameter> mRefundPolicyParameters;
        public virtual IEnumerable<RefundPolicyParameter> RefundPolicyParameters => mRefundPolicyParameters;
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public CredentialApplicationRefundPolicy()
        {
            mRefundPolicyParameters = new List<RefundPolicyParameter>();
        }
    }
}
