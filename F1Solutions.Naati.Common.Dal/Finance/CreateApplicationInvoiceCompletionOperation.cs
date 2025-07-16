using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Wiise.PublicModels;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public class CreateApplicationInvoiceCompletionOperation : CreateInvoiceCompletionOperation
    {
        public int CredentialWorkflowFeeId { get; set; }
        protected override void PerformOperation(Invoice invoice)
        {
            LoggingHelper.LogDebug("Performing CreateApplicationInvoiceCompletionOperation for invoice {InvoiceNumber}", invoice?.InvoiceNumber);

            CredentialWorkflowFeeId.Requires(x => x > 0, "CreateApplicationInvoiceCompletionOperation must have a CredentialWorkflowFeeId");

            var fee = NHibernateSession.Current.Get<CredentialWorkflowFee>(CredentialWorkflowFeeId);
            fee.InvoiceNumber = invoice.InvoiceNumber;
            fee.InvoiceId = invoice.InvoiceID;

            NHibernateSession.Current.Save(fee);
            NHibernateSession.Current.Flush();
        }
    }
}
