namespace F1Solutions.Naati.Common.Contracts.Bl.Refund
{
    public interface ICredentialApplicationRefundService
    {
        /// <summary>
        /// Validates the processed date for a paypal payment is within 6 months.
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <returns>true if payment was within 180 days, otherwise false.</returns>
        bool ValidatePayPalDateForRefund(int credentialRequestId);
    }
}
