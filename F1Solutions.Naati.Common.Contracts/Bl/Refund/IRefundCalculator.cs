namespace F1Solutions.Naati.Common.Contracts.Bl.Refund
{
    public interface IRefundCalculator
    {
        RefundCalculationDto CalculateRefund(int credentialRequestId);
        bool ValidateRefundRequest(int credentialRequestId);
    }

    public enum RefundCalculationResultTypeName : int
    {
        NotCalculated = 0,
        RefundAvailable = 1,
        NoRefund = 2,
    }
}
