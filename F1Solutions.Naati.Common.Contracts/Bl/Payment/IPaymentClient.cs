namespace F1Solutions.Naati.Common.Contracts.Bl.Payment
{
    public interface IPaymentClient
    {
       // PaymentResponse MakePayment(PaymentRequest request);
        PaymentResponse MakePaymentEmbedded(PaymentRequestEmbedded request);
        PaymentResponse MakeRefundPayment(RefundPaymentRequest request);
        PaymentResponse MakeRefundPaymentEmbedded(RefundPaymentRequest request);
    }
}
