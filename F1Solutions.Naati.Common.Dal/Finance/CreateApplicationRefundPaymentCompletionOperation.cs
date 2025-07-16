using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Wiise.PublicModels;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public class CreateApplicationRefundPaymentCompletionOperation : CreatePaymentCompletionOperation
    {
        public int CredentialApplicationRefundId { get; set; }
        protected override void PerformOperation(Payment payment)
        {
            LoggingHelper.LogDebug("Performing {Operation} for payment {PaymentReference}", nameof(CreateApplicationRefundPaymentCompletionOperation), payment?.Reference);

            CredentialApplicationRefundId.Requires(x => x > 0, $"{nameof(CreateApplicationRefundPaymentCompletionOperation)} must have a {nameof(CredentialApplicationRefundId)}");

            var refund = NHibernateSession.Current.Get<CredentialApplicationRefund>(CredentialApplicationRefundId);
            refund.PaymentReference = payment.Reference;
            refund.CreditNoteNumber = payment.CreditNoteNumber; 

            NHibernateSession.Current.Save(refund);
            NHibernateSession.Current.Flush();
        }
    }
}