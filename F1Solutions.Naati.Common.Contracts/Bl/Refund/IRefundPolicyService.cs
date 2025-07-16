using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal;

namespace F1Solutions.Naati.Common.Contracts.Bl.Refund
{
    public interface IRefundPolicyService
    {
        double? CalculateRefund(CalculateRefundInputData calculateRefundRequest); 
    }

    //public class CalculateRefundRequest
    //{
    //    public int CredentialWorkflowFeeId { get; set; }
    //    public int CredntialRequestStatusTypeId { get; set; }
    //    public int CredentialTypeId { get; set; }
    //    public string PaymentReference { get; set; }
    //    public IEnumerable<CredentialRequestTestSession> TestSessions { get; set; }
    //    public string Policy { get; set; }
    //    public DateTime? PaymentActionProcessedDate { get; set; }
    //}

    public class CredentialRequestTestSession
    {
        public bool Rejected { get; set; }
        public bool Sat { get; set; }
        public DateTime TestDate { get; set; }
    }
}
