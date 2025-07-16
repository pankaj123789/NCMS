using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;


namespace F1Solutions.Naati.Common.Dal.Finance
{
    public class CreateApplicationRefundCompletionOperation : WiiseCompletionOperation
    {
        public int CredentialApplicationRefundId { get; set; }
        public override void PerformOperation(object wiiseOperationResult)
        {
            var creditNote = GetCreditNote(wiiseOperationResult);
            LoggingHelper.LogDebug("Performing {Operation} for credit note {CreditNoteNumber}", nameof(CreateApplicationRefundCompletionOperation), creditNote?.CreditNoteNumber);


            if (creditNote == null)
            {
                throw new Exception("CreditNote not provided.");
            }

            CredentialApplicationRefundId.Requires(x => x > 0, $"{nameof(CreateApplicationRefundCompletionOperation)} must have a CredentiaRefundId");

            var refund = NHibernateSession.Current.Get<CredentialApplicationRefund>(CredentialApplicationRefundId);
            refund.CreditNoteNumber = creditNote.CreditNoteNumber;
            refund.CreditNoteId = creditNote.CreditNoteID;

            NHibernateSession.Current.Save(refund);
            NHibernateSession.Current.Flush();
        }

        private CreditNote GetCreditNote(object wiiseOperationResult)
        {
            var creditNote = wiiseOperationResult as CreditNote;

            if (creditNote == null)
            {
                throw new Exception("CreditNote not provided.");
            }

            return creditNote;
        }
    }
}
