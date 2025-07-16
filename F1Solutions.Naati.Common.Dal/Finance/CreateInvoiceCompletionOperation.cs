using System;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Wiise.PublicModels;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public abstract class CreateInvoiceCompletionOperation : WiiseCompletionOperation
    {
        public override void PerformOperation(object operationResult)
        {
            var invoice = operationResult as Invoice;

            if (invoice == null)
            {
                throw new Exception("Invoice not provided.");
            }

            PerformOperation(invoice);
        }

        protected abstract void PerformOperation(Invoice invoice);
    }
}
