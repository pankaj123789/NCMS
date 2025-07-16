using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System.Text.RegularExpressions;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public class CreateApplicationPaymentCompletionOperation : CreatePaymentCompletionOperation
    {
        public int CredentialWorkflowFeeId { get; set; }
        protected override void PerformOperation(Payment payment)
        {
            LoggingHelper.LogDebug("Performing CreateApplicationPaymentCompletionOperation for payment {PaymentReference}", payment?.Reference);

            CredentialWorkflowFeeId.Requires(x => x > 0, "CreateApplicationPaymentCompletionOperation must have a CredentialWorkflowFeeId");

            var fee = NHibernateSession.Current.Get<CredentialWorkflowFee>(CredentialWorkflowFeeId);
            //need to truncate the Paypal Code to handle just PAYPAL (as in PAYID-L66GNKY1KR77349W9689023S - PAYPAL-9NB41868YH365790K)
            if (payment.Reference.StartsWith(PayPalService.PAYPALORDERPREFIX))
            {
                Match m = Regex.Match(payment.Reference, "(PAYPAL.*)", RegexOptions.IgnoreCase);
                fee.PaymentReference = m.Value;
            }
            else
            {
                fee.PaymentReference = payment.Reference;
            }
            fee.InvoiceId = payment.InvoiceId;
            fee.InvoiceNumber = payment.InvoiceNumber; 

            NHibernateSession.Current.Save(fee);
            NHibernateSession.Current.Flush();
        }
    }
}