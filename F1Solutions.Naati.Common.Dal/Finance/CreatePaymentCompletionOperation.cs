using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public abstract class CreatePaymentCompletionOperation : WiiseCompletionOperation
    {
        public override void PerformOperation(object operationResult)
        {
            var payment = (operationResult as Payment);
            var payments = ((operationResult as Payments) ?? new Payments());
            if (payment != null)
            {
                payments._Payments.Add(payment);
            }
            if (!payments._Payments.Any())
            {
                throw new Exception("Payment not provided.");
            }

            foreach (var p in payments._Payments)
            {
                PerformOperation(p);
            }
            
        }

        protected abstract void PerformOperation(Payment payment);
    }
}