using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CredentialApplicationRefundPolicyData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<RefundPolicyParameterData> RefundPolicyParameters { get; set; }
    }
}
